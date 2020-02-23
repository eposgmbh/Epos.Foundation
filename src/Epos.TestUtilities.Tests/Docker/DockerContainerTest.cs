using System.Linq;

using NUnit.Framework;

namespace Epos.TestUtilities.Docker
{
    [TestFixture]
    [Ignore("Does not work on Azure DevOps.")]
    public sealed class DockerContainerTest
    {
        [Test]
        public void Basics()
        {
            var theContainer = DockerContainer.StartAndWaitForReadynessLogPhrase(
                new DockerContainerOptions
                {
                    Name = "Gabbel123",
                    ImageName = "postgres:latest",
                    ReadynessLogPhrase = "ready to accept connections",
                    Ports = { (31000, 5432) }
                }
            );

            Assert.That(theContainer.Id, Is.Not.Null);
            Assert.That(theContainer.Name, Is.EqualTo("Gabbel123"));

            // Do it once more to assert that the container is force removed before the start.

            theContainer = DockerContainer.StartAndWaitForReadynessLogPhrase(
                new DockerContainerOptions
                {
                    Name = "Gabbel123",
                    ImageName = "postgres:latest",
                    ReadynessLogPhrase = "ready to accept connections",
                    Ports = { (31000, 5432) }
                }
            );

            Assert.That(theContainer.Id, Is.Not.Null);
            Assert.That(theContainer.Name, Is.EqualTo("Gabbel123"));

            theContainer.ForceRemove();
        }

        [Test]
        public void BuiltinContainers()
        {
            var theContainer = DockerContainer.Postgres.Start();

            Assert.That(theContainer.Id, Is.Not.Null);
            Assert.That(theContainer.Name, Is.EqualTo("PostgresTestContainer"));

            var (thePort, _) = theContainer.Ports.First();

            Assert.That(
                theContainer.ConnectionString,
                Is.EqualTo($"Server=localhost;Port={thePort};Database=test;User ID=admin;Password=admin")
            );

            theContainer.ForceRemove();

            // ----------------------------------------------------------

            theContainer = DockerContainer.SqlServer.Start();

            Assert.That(theContainer.Id, Is.Not.Null);
            Assert.That(theContainer.Name, Is.EqualTo("SqlServerTestContainer"));

            (thePort, _) = theContainer.Ports.First();

            Assert.That(
                theContainer.ConnectionString,
                Is.EqualTo($"Server=localhost,{thePort};Database=test;User Id=SA;Password=aB1cD2eF3")
            );

            theContainer.ForceRemove();

            // ----------------------------------------------------------

            theContainer = DockerContainer.MongoDB.Start();

            Assert.That(theContainer.Id, Is.Not.Null);
            Assert.That(theContainer.Name, Is.EqualTo("MongoDBTestContainer"));

            (thePort, _) = theContainer.Ports.First();

            Assert.That(theContainer.ConnectionString, Is.EqualTo($"mongodb://localhost:{thePort}"));

            theContainer.ForceRemove();
        }
    }
}
