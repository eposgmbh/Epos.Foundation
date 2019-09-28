using System;
using System.Reflection;

using NUnit.Framework;

using System.Linq;
using System.Collections.Generic;

namespace Epos.Utilities
{
    [TestFixture]
    public class AssemblyExtensionsTest
    {
        [Test]
        public void HasAttribute() {
            Assert.Throws<ArgumentNullException>(
                () =>AssemblyExtensions.HasAttribute<AssemblyFileVersionAttribute>(null!)
            );
            Assert.That(typeof(AssemblyExtensionsTest).Assembly.HasAttribute<AssemblyFileVersionAttribute>());
            Assert.That(typeof(AssemblyExtensionsTest).Assembly.HasAttribute<AttributeUsageAttribute>(), Is.False);
        }

        [Test]
        public void GetAttribute() {
            Assert.Throws<ArgumentNullException>(() => AssemblyExtensions.GetAttribute<AssemblyTitleAttribute>(null!));

            AssemblyTitleAttribute theAssemblyTitleAttribute =
                Assembly.GetExecutingAssembly().GetAttribute<AssemblyTitleAttribute>();
            Assert.That(theAssemblyTitleAttribute.Title, Is.EqualTo("Epos.Utilities.Tests"));

            AttributeUsageAttribute theUnknownAttribute =
                Assembly.GetExecutingAssembly().GetAttribute<AttributeUsageAttribute>();
            Assert.That(theUnknownAttribute, Is.Null);
        }

        [Test]
        public void GetAttributes() {
            Assert.Throws<ArgumentNullException>(() => AssemblyExtensions.GetAttributes<AssemblyTitleAttribute>(null!));

            IEnumerable<AssemblyTitleAttribute> theAssemblyTitleAttributes =
                Assembly.GetExecutingAssembly().GetAttributes<AssemblyTitleAttribute>();
            Assert.That(theAssemblyTitleAttributes.Single().Title, Is.EqualTo("Epos.Utilities.Tests"));

            IEnumerable<AttributeUsageAttribute> theUnknownAttributes =
                Assembly.GetExecutingAssembly().GetAttributes<AttributeUsageAttribute>();
            Assert.That(theUnknownAttributes, Is.Empty);
        }
    }
}
