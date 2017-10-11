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
        public void Basics() {
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
    }
}