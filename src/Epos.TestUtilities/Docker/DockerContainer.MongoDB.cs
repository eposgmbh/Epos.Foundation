namespace Epos.TestUtilities.Docker
{
    public partial class DockerContainer
    {
        /// <summary> Represents a MongoDB Docker container (no auth). </summary>
        public static class MongoDB
        {
            /// <summary> Starts a MongoDB Docker container. </summary>
            /// <returns>Docker container</returns>
            public static DockerContainer Start() {
                int thePort = GetFreeTcpHostPort();

                var theContainer = StartAndWaitForReadynessLogPhrase(
                    new DockerContainerOptions
                    {
                        Name = "MongoDBTestContainer",
                        ImageName = "mongo:latest",
                        ReadynessLogPhrase = "waiting for connections",
                        Ports = { (thePort, 27017) }
                    }
                );

                theContainer.ConnectionString = $"mongodb://localhost:{thePort}";

                return theContainer;
            }
        }
    }
}
