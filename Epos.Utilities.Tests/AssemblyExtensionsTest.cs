using System.Reflection;

using NUnit.Framework;

using System.Linq;

namespace Epos.Utilities
{
    [TestFixture]
    public class AssemblyExtensionsTest
    {
        [Test]
        public void HasAttribute() {
            Assert.That(typeof(AssemblyExtensionsTest).Assembly.HasAttribute<AssemblyFileVersionAttribute>());
        }

        [Test]
        public void GetAttributes() {
            var theAssemblyTitleAttributes = Assembly.GetExecutingAssembly().GetAttributes<AssemblyTitleAttribute>();
            Assert.That(theAssemblyTitleAttributes.Single().Title, Is.EqualTo("Epos.Utilities.Tests"));
        }
    }
}