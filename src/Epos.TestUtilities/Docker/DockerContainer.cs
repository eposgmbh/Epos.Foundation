using System;
using System.Diagnostics;
using System.Text;

namespace Epos.TestUtilities.Docker
{
    /// <summary> Represents a Docker container that can be started and stopped. </summary>
    public sealed class DockerContainer
    {
        /// <summary> Starts a container and optionally waits for a readyness log phrase. </summary>
        /// <param name="options">Options</param>
        /// <returns>Docker container</returns>
        public static DockerContainer StartAndWaitForOpenPort(DockerContainerOptions options) {
            if (options == null) {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.ImageName == null) {
                throw new ArgumentNullException("options.ImageName");
            }

            var theDockerArguments = new StringBuilder(
                $"run --detach --publish {options.HostPort}:{options.ContainerPort}"
            );

            foreach ((string key, string value) in options.EnvironmentVariables) {
                theDockerArguments
                    .Append(" -e \"").Append(key)
                    .Append("=").Append(value).Append("\"");
            }

            theDockerArguments
                .Append(' ').Append(options.ImageName);

            string theContainerId = ExecuteDockerCli(theDockerArguments.ToString()).Substring(0, 10);

            var theContainer = new DockerContainer(theContainerId, options);

            WaitForReadyContainer(theContainer);

            return theContainer;
        }

        private DockerContainer(string id, DockerContainerOptions options) {
            Id = id;
            StartOptions = options;
        }

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

        /// <summary> Gets the host port. </summary>
        public int HostPort => StartOptions.HostPort;

        /// <summary> Gets the container port. </summary>
        public int ContainerPort => StartOptions.ContainerPort;

        /// <summary> Force removes the container. </summary>
        public void ForceRemove() => ExecuteDockerCli($"rm --volumes --force {Id}");

        #region --- Helper methods ---

        private static string ExecuteDockerCli(string dockerArguments) {
            var p = new Process();

            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
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

                if (theLogs.Contains(theReadynessLogPhrase)) {
                    return;
                }
            }

            container.ForceRemove();

            throw new DockerContainerException(
                $"Timeout waiting for container readyness after {theTimeout} ms ({theReadynessLogPhrase})."
            );
        }

        #endregion
    }
}
