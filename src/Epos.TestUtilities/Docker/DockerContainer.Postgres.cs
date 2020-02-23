namespace Epos.TestUtilities.Docker
{
    public partial class DockerContainer
    {
        /// <summary> Represents a Postgres Docker container (with admin/admin auth). </summary>
        public static class Postgres
        {
            /// <summary> Starts a Postgres Docker container. </summary>
            /// <returns>Docker container</returns>
            public static DockerContainer Start() {
                int thePort = GetFreeTcpHostPort();

                var theContainer = StartAndWaitForReadynessLogPhrase(
                    new DockerContainerOptions
                    {
                        Name = "PostgresTestContainer",
                        ImageName = "postgres:latest",
                        ReadynessLogPhrase = "ready to accept connections",
                        Ports = { (thePort, 5432) },
                        EnvironmentVariables = {
                            ("POSTGRES_USER", "admin"),
                            ("POSTGRES_PASSWORD", "admin")
                        }
                    }
                );

                theContainer.ConnectionString =
                    $"Server=localhost;Port={thePort};Database=test;User ID=admin;Password=admin";

                return theContainer;
            }
        }
    }
}
