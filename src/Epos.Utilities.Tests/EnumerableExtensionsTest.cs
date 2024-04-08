using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using System.Linq;

namespace Epos.Utilities;

[TestFixture]
public class EnumerableExtensionsTest
{
    [Test]
    public void ToEnumerable() {
        IEnumerable<int> theInt32Enumerable = 1.ToEnumerable();
        Assert.That(theInt32Enumerable.Single(), Is.EqualTo(1));

        IEnumerable<string> theStringEnumerable = "Hello".ToEnumerable();
        Assert.That(theStringEnumerable.Single(), Is.EqualTo("Hello"));

        IEnumerable<DateTime> theDateTimeEnumerable = new DateTime(2011, 04, 22).ToEnumerable();
        Assert.That(theDateTimeEnumerable.Single(), Is.EqualTo(new DateTime(2011, 04, 22)));

    }

    [Test]
    public void ForEach() {
        string[] theStringEnumerable = ["This ", "is ", "a ", "test."];

        var theBuilder = new StringBuilder();
        theStringEnumerable.ForEach(s => theBuilder.Append(s));

        Assert.That(theBuilder.ToString(), Is.EqualTo("This is a test."));

#if DEBUG
        Assert.Throws<ArgumentNullException>(() => theStringEnumerable.ForEach(null!));

        Assert.Throws<ArgumentNullException>(
            () => EnumerableExtensions.ForEach(null!, (Action<object>) (obj => { }))
        );
#endif
    }

    [Test]
    public void FlattenRecursiveHierarchy() {
        var theChild = new Child("Hermann") {
            Children = {
                new Child("Jan") {
                    Children = {
                        new Child("Jakob"),
                        new Child("Frederick")
                    }
                }
            }
        };

        var theList = theChild.ToEnumerable().FlattenRecursiveHierarchy(c => c.Children).ToList();
        Assert.That(theList.Count, Is.EqualTo(4));
        Assert.That(theList.First().Name, Is.EqualTo("Hermann"));
        Assert.That(theList.Last().Name, Is.EqualTo("Frederick"));
    }

    private sealed class Child
    {
        public Child(string name) {
            Name = name;
            Children = new List<Child>();
        }

        public string Name { get; }

        public IList<Child> Children { get; }
    }
}
