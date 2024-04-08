using System;

namespace Epos.TestUtilities.Docker;

public partial class DockerContainer
{
    /// <summary> Represents a MongoDB Docker container (no auth). </summary>
    public static class MongoDB
    {
        /// <summary> Latest definitely supported version </summary>
        public const string LatestVersion = "6.0";

        /// <summary> Starts a MongoDB Docker container. </summary>
        /// <returns>Docker container</returns>
        public static DockerContainer Start(string version = LatestVersion) {
            if (string.IsNullOrWhiteSpace(version)) {
                throw new ArgumentException($"\"{nameof(version)}\" must not be null or white space.", nameof(version));
            }

            int thePort = GetFreeTcpHostPort();

            var theContainer = StartAndWaitForReadynessLogPhrase(
                new DockerContainerOptions
                {
                    Name = "MongoDBTestContainer",
                    ImageName = $"mongo:{version.Trim()}",
                    ReadynessLogPhrase = "Waiting for connections",
                    Ports = { (thePort, 27017) }
                }
            );

            theContainer.ConnectionString = $"mongodb://localhost:{thePort}";

            return theContainer;
        }
    }
}
