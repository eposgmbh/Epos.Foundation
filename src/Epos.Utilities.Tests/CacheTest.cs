using System;

using NUnit.Framework;

namespace Epos.Utilities
{
    [TestFixture]
    public class CacheTest
    {
        [Test]
        public void ConstructorExceptions() =>
            Assert.Throws<ArgumentOutOfRangeException>(() => { new Cache<object, object>(1); });

        [Test]
        public void ConstructorAndBasics() {
            var theCache = new Cache<int, string>(2);
            Assert.That(theCache.Capacity, Is.EqualTo(2));
            Assert.That(theCache.Count, Is.EqualTo(0));

            theCache[1] = "One";
            Assert.That(theCache.Capacity, Is.EqualTo(2));
            Assert.That(theCache.Count, Is.EqualTo(1));
            Assert.That(theCache[1], Is.EqualTo("One"));

            theCache[1] = "Eins";
            Assert.That(theCache.Capacity, Is.EqualTo(2));
            Assert.That(theCache.Count, Is.EqualTo(1));
            Assert.That(theCache[1], Is.EqualTo("Eins"));

            theCache[2] = "Zwei";
            Assert.That(theCache.Capacity, Is.EqualTo(2));
            Assert.That(theCache.Count, Is.EqualTo(2));
            Assert.That(theCache[1], Is.EqualTo("Eins"));
            Assert.That(theCache[2], Is.EqualTo("Zwei"));

            theCache[3] = "Drei";
            Assert.That(theCache.Capacity, Is.EqualTo(2));
            Assert.That(theCache.Count, Is.EqualTo(2));
            Assert.That(theCache[1], Is.Null);
            Assert.That(theCache[2], Is.EqualTo("Zwei"));
            Assert.That(theCache[3], Is.EqualTo("Drei"));

            Assert.That(theCache.ToString(), Is.EqualTo("[{ 2: Zwei }, { 3: Drei }]"));
        }
    }
}
