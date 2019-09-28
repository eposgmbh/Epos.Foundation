using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace Epos.Utilities
{
    [TestFixture]
    public class ListExtensionsTest
    {
        [Test]
        public void EqualsList() {
            var theList1 = new List<int> { 1, 2, 3, 4, 5};
            var theList2 = new List<int> { 1, 2, 3, 4, 6};
            var theList3 = new List<int> { 1, 2, 4, 5};
            var theList4 = new List<int> { 1, 2, 4, 5};

            Assert.That(theList1.EqualsList(theList1));
            Assert.That(!theList1.EqualsList(theList2));
            Assert.That(!theList1.EqualsList(theList3));
            Assert.That(!theList1.EqualsList(theList4));

            Assert.That(theList2.EqualsList(theList2));
            Assert.That(!theList2.EqualsList(theList3));
            Assert.That(!theList2.EqualsList(theList4));

            Assert.That(theList3.EqualsList(theList3));
            Assert.That(theList3.EqualsList(theList4));

            Assert.That(theList4.EqualsList(theList4));

            Assert.Throws<ArgumentNullException>(() => ListExtensions.EqualsList(null!, theList1));
            Assert.Throws<ArgumentNullException>(() => theList1.EqualsList(null!));
        }

        [Test]
        public void GetListHashCode() {
            var theDoubles = new List<double> { 1.0, 2.0, 99.0, 101.5 };
            Assert.That(theDoubles.GetListHashCode(), Is.EqualTo(308829609));

            var theStrings = new List<string> { "This", "is", "a", "test" };
            Assert.That(theStrings.GetListHashCode(), Is.Not.EqualTo(0));

            var theStringsWithNull = new List<string?> { "This", "is", "a", "test", "with", null };
            Assert.That(theStringsWithNull.GetListHashCode(), Is.Not.EqualTo(0));

            Assert.Throws<ArgumentNullException>(() => (null as List<object>)!.GetListHashCode());
        }
    }
}
