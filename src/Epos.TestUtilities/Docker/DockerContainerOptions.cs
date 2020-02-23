using System.Collections.Generic;

namespace Epos.TestUtilities.Docker
{
    /// <summary> Specifies options for container start. </summary>
    public sealed class DockerContainerOptions
    {
        /// <summary> Gets or sets the container name. </summary>
        public string Name { get; set;}

        /// <summary> Gets or sets the container image name. </summary>
        public string ImageName { get; set; }

        /// <summary> Gets the port list. </summary>
        public List<(int hostPort, int containerPort)> Ports { get; } = new List<(int hostPort, int containerPort)>();

        /// <summary> Gets the environment variable list. </summary>
        public List<(string key, string value)> EnvironmentVariables { get; } =
            new List<(string key, string value)>();

        /// <summary> Gets or sets the hostname. </summary>
        public string Hostname { get; set; }

        /// <summary> Gets or sets the readyness log phrase. </summary>
        public string ReadynessLogPhrase { get; set; }

        /// <summary> Gets or sets the readyness log phrase timeout (default: 30000 ms). </summary>
        public int ReadynessLogPhraseTimeout { get; set; } = 30000;
    }
}
