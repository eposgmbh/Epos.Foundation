namespace Epos.TestUtilities.Docker;

public partial class DockerContainer
{
    /// <summary> Represents a SQL Server Docker container (with SA/aB1cD2eF3 auth). </summary>
    public static class SqlServer
    {
        /// <summary> Starts a SQL Server Docker container. </summary>
        /// <returns>Docker container</returns>
        public static DockerContainer Start() {
            int thePort = GetFreeTcpHostPort();

            var theContainer = StartAndWaitForReadynessLogPhrase(
                new DockerContainerOptions
                {
                    Name = "SqlServerTestContainer",
                    ImageName = "mcr.microsoft.com/mssql/server:2019-latest",
                    ReadynessLogPhrase = "Service Broker manager has started",
                    ReadynessLogPhraseTimeout = 60000,
                    Ports = { (thePort, 1433) },
                    EnvironmentVariables = {
                        ("ACCEPT_EULA", "Y"),
                        ("SA_PASSWORD", "aB1cD2eF3")
                    }
                }
            );

            theContainer.ConnectionString =
                $"Server=localhost,{thePort};Database=test;User Id=SA;Password=aB1cD2eF3";

            return theContainer;
        }
    }
}
