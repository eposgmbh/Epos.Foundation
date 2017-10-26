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
            Assert.That(typeof(AssemblyExtensionsTest).Assembly.HasAttribute<AttributeUsageAttribute>(), Is.False);
        }

        [Test]
        public void GetAttribute() {
            Assert.Throws<ArgumentNullException>(() => AssemblyExtensions.GetAttribute<AssemblyTitleAttribute>(null));

            var theAssemblyTitleAttribute = Assembly.GetExecutingAssembly().GetAttribute<AssemblyTitleAttribute>();
            Assert.That(theAssemblyTitleAttribute.Title, Is.EqualTo("Epos.Utilities.Tests"));

            var theUnknownAttribute = Assembly.GetExecutingAssembly().GetAttribute<AttributeUsageAttribute>();
            Assert.That(theUnknownAttribute, Is.Null);
        }

        [Test]
        public void GetAttributes() {
            Assert.Throws<ArgumentNullException>(() => AssemblyExtensions.GetAttributes<AssemblyTitleAttribute>(null));

            var theAssemblyTitleAttributes = Assembly.GetExecutingAssembly().GetAttributes<AssemblyTitleAttribute>();
            Assert.That(theAssemblyTitleAttributes.Single().Title, Is.EqualTo("Epos.Utilities.Tests"));

            var theUnknownAttributes = Assembly.GetExecutingAssembly().GetAttributes<AttributeUsageAttribute>();
            Assert.That(theUnknownAttributes, Is.Empty);
        }
    }
}