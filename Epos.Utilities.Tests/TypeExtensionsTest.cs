using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using NUnit.Framework;

namespace Epos.Utilities
{
    [TestFixture]
    public class TypeExtensionsTest
    {
        [Test]
        public void GetGenericMethod() {
            Assert.Throws<ArgumentNullException>(
                () => TypeExtensions.GetGenericMethod(null, "0", BindingFlags.Default, new Type[] { }, new Type[] { })
            );
            Assert.Throws<ArgumentNullException>(
                () => typeof(int).GetGenericMethod(null, BindingFlags.Default, new Type[] { }, new Type[] { })
            );
            Assert.Throws<ArgumentNullException>(
                () => typeof(int).GetGenericMethod("0", BindingFlags.Default, null, new Type[] { })
            );
            Assert.Throws<ArgumentNullException>(
                () => typeof(int).GetGenericMethod("0", BindingFlags.Default, new Type[] { }, null)
            );

            var theMethodInfo = typeof(Enumerable).GetGenericMethod(
                "Empty", BindingFlags.Static | BindingFlags.Public,
                new[] { typeof(string) }, new Type[] { }
            );
            Assert.That(theMethodInfo, Is.Not.Null);

            theMethodInfo = typeof(Enumerable).GetGenericMethod(
                "Single", BindingFlags.Static | BindingFlags.Public,
                new[] { typeof(int) }, new[] { typeof(IEnumerable<int>) }
            );
            Assert.That(theMethodInfo, Is.Not.Null);
        }

        [Test]
        public void HasAttribute() {
            Assert.Throws<ArgumentNullException>(() => TypeExtensions.HasAttribute<AttributeUsageAttribute>(null));

            Assert.That(typeof(AssemblyCompanyAttribute).HasAttribute<AttributeUsageAttribute>(), Is.True);
            Assert.That(typeof(int).HasAttribute<AttributeUsageAttribute>(), Is.False);
        }

        [Test]
        public void GetAttribute()
        {
            Assert.Throws<ArgumentNullException>(() => TypeExtensions.GetAttribute<AttributeUsageAttribute>(null));

            Assert.That(typeof(AssemblyCompanyAttribute).GetAttribute<AttributeUsageAttribute>(), Is.Not.Null);
            Assert.That(typeof(AssemblyCompanyAttribute).GetAttribute<AttributeUsageAttribute>().ValidOn, Is.EqualTo(AttributeTargets.Assembly));
            Assert.That(typeof(int).GetAttribute<AttributeUsageAttribute>(), Is.Null);
        }
    }
}
