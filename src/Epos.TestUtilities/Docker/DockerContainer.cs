using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Epos.TestUtilities.Docker
{
    /// <summary> Represents a Docker container that can be started and stopped. </summary>
    public sealed partial class DockerContainer : IDisposable
    {
        /// <summary> Starts a container and optionally waits for a readyness log phrase. </summary>
        /// <param name="options">Options</param>
        /// <returns>Docker container</returns>
        public static DockerContainer StartAndWaitForReadynessLogPhrase(DockerContainerOptions options) {
            if (options == null) {
                throw new ArgumentNullException(nameof(options));
            }

            if (string.IsNullOrEmpty(options.Name)) {
                throw new ArgumentException("options.Name must not be null or empty.");
            }

            if (options.ImageName == null) {
                throw new ArgumentNullException("options.ImageName");
            }

            var theDockerArguments = new StringBuilder($"run --detach --name ").Append(options.Name);

            if (options.Hostname != null) {
                theDockerArguments.Append($" --hostname {options.Hostname}");
            }

            foreach ((int hostPort, int containerPort) in options.Ports) {
                theDockerArguments.Append($" --publish {hostPort}:{containerPort}");
            }

            foreach ((string key, string value) in options.EnvironmentVariables) {
                theDockerArguments.Append($" -e \"{key}={value}\"");
            }

            theDockerArguments.Append(' ').Append(options.ImageName);

            ExecuteDockerCli($"rm --volumes --force {options.Name}");

            string theContainerId = ExecuteDockerCli(theDockerArguments.ToString()).Substring(0, 8);

            var theContainer = new DockerContainer(theContainerId, options);

            WaitForReadyContainer(theContainer);

            return theContainer;
        }

        private DockerContainer(string id, DockerContainerOptions options) {
            Id = id;
            StartOptions = options;
        }

        /// <summary> Destroys (force removes) the container. </summary>
        ~DockerContainer() => ForceRemove();

        /// <summary> Gets the container start options </summary>
        public DockerContainerOptions StartOptions { get; }

        /// <summary> Gets the container id. </summary>
        public string Id { get; }

        /// <summary> Gets the container name. </summary>
        public string Name => ExecuteDockerCli($"ps --format \"{{{{.Names}}}}\" --filter \"id={Id}\"");

        /// <summary> Gets the container status. </summary>
        public string Status => ExecuteDockerCli($"ps --format \"{{{{.Status}}}}\" --filter \"id={Id}\"");

        /// <summary> Gets the container image name. </summary>
        public string ImageName => StartOptions.ImageName;

        /// <summary> Gets the port list. </summary>
        public List<(int hostPort, int containerPort)> Ports => StartOptions.Ports;

        /// <summary> Returns a connection string, if applicable. </summary>
        public string ConnectionString { get; internal set; }

        /// <summary> Force removes the container. </summary>
        public void ForceRemove() => ExecuteDockerCli($"rm --volumes --force {Id}");

        void IDisposable.Dispose() => ForceRemove();

        #region --- Helper methods ---

        private static string ExecuteDockerCli(string dockerArguments) {
            var p = new Process();

            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.FileName = "docker";
            p.StartInfo.Arguments = dockerArguments;

            try {
                p.Start();
            } catch (Exception theException) {
                throw new DockerContainerException(
                    $"Error running Docker command line: docker {dockerArguments}", theException
                );
            }

            string theResult = p.StandardOutput.ReadToEnd().Trim();
            p.WaitForExit();

            return theResult;
        }

        private static void WaitForReadyContainer(DockerContainer container) {
            string theReadynessLogPhrase = container.StartOptions.ReadynessLogPhrase;

            if (string.IsNullOrEmpty(theReadynessLogPhrase)) {
                return;
            }

            var theStopwatch = Stopwatch.StartNew();
            int theTimeout = container.StartOptions.ReadynessLogPhraseTimeout;

            while (theStopwatch.ElapsedMilliseconds < theTimeout) {
                string theLogs = ExecuteDockerCli($"logs {container.Id}");

                if (theLogs.IndexOf(theReadynessLogPhrase, StringComparison.InvariantCultureIgnoreCase) != -1) {
                    return;
                }
            }

            container.ForceRemove();

            throw new DockerContainerException(
                $"Timeout waiting for container readyness after {theTimeout} ms ({theReadynessLogPhrase})."
            );
        }

        private static int GetFreeTcpHostPort() {
            TcpListener theTcpListener = new TcpListener(IPAddress.Loopback, 0);

            theTcpListener.Start();
            int thePort = ((IPEndPoint) theTcpListener.LocalEndpoint).Port;
            theTcpListener.Stop();

            return thePort;
        }

        #endregion
    }
}
