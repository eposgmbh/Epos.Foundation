using System.Collections.Generic;
using NUnit.Framework;

namespace Epos.Utilities;

[TestFixture]
public class DictionaryExtensionsTest
{
    [Test]
    public void Get() {
        var theDictionary = new Dictionary<int, string?> {
            { 33, "Thirty three" },
            { 44, "Forty four" }
        };

        Assert.That(theDictionary.Get(33), Is.EqualTo("Thirty three"));
        Assert.That(theDictionary.Get(44), Is.EqualTo("Forty four"));
        Assert.That(theDictionary.Get(55), Is.Null);
    }
}
