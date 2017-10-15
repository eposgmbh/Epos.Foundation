using System;
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
            Assert.Throws<ArgumentNullException>(() =>AssemblyExtensions.HasAttribute<AssemblyFileVersionAttribute>(null));
            Assert.That(typeof(AssemblyExtensionsTest).Assembly.HasAttribute<AssemblyFileVersionAttribute>());
        }

        [Test]
        public void GetAttribute() {
            Assert.Throws<ArgumentNullException>(() => AssemblyExtensions.GetAttribute<AssemblyTitleAttribute>(null));

            var theAssemblyTitleAttribute = Assembly.GetExecutingAssembly().GetAttribute<AssemblyTitleAttribute>();
            Assert.That(theAssemblyTitleAttribute.Title, Is.EqualTo("Epos.Utilities.Tests"));
        }

        [Test]
        public void GetAttributes() {
            Assert.Throws<ArgumentNullException>(() => AssemblyExtensions.GetAttributes<AssemblyTitleAttribute>(null));

            var theAssemblyTitleAttributes = Assembly.GetExecutingAssembly().GetAttributes<AssemblyTitleAttribute>();
            Assert.That(theAssemblyTitleAttributes.Single().Title, Is.EqualTo("Epos.Utilities.Tests"));
        }
    }
}