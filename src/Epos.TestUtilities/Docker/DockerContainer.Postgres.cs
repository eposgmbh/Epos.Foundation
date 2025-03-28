using System;

namespace Epos.TestUtilities.Docker;

public partial class DockerContainer
{
    /// <summary> Represents a Postgres Docker container (with admin/admin auth). </summary>
    public static class Postgres
    {
        /// <summary> Latest definitely supported version </summary>
        public const string LatestVersion = "14.5";

        /// <summary> Starts a Postgres Docker container. </summary>
        /// <returns>Docker container</returns>
        public static DockerContainer Start(string version = LatestVersion) {
            if (string.IsNullOrWhiteSpace(version)) {
                throw new ArgumentException($"\"{nameof(version)}\" must not be null or white space.", nameof(version));
            }

            int thePort = GetFreeTcpHostPort();

            DockerContainer theContainer = StartAndWaitForReadynessLogPhrase(
                new DockerContainerOptions
                {
                    Name = "PostgresTestContainer",
                    ImageName = $"postgres:{version}",
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
