using System;

namespace Epos.TestUtilities.Docker;

public partial class DockerContainer
{
    /// <summary> Represents a RabbitMQ Docker container (with guest/guest auth). </summary>
    public static class RabbitMQ
    {
        /// <summary> Latest definitely supported version </summary>
        public const string LatestVersion = "3.7.4-management-alpine";

        /// <summary> Starts a RabbitMQ Docker container. </summary>
        /// <returns>Docker container</returns>
        public static DockerContainer Start(string version = LatestVersion) {
            if (string.IsNullOrWhiteSpace(version)) {
                throw new ArgumentException($"\"{nameof(version)}\" must not be null or white space.", nameof(version));
            }

            return StartAndWaitForReadynessLogPhrase(
                new DockerContainerOptions
                {
                    Name = "RabbitMQTestContainer",
                    ImageName = $"rabbitmq:{version}",
                    Hostname = "rabbitmq-host",
                    Ports = {
                        (hostPort: 5672, containerPort: 5672),
                        (hostPort: 15672, containerPort: 15672)
                    },
                    ReadynessLogPhrase = "startup complete"
                }
            );
        }
    }
}
