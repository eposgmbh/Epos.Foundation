using System;
using System.Globalization;
using NUnit.Framework;

namespace Epos.Utilities;

[TestFixture]
public class StringExtensionsTest
{
    [Test]
    public void Extract() {
        Assert.Throws<ArgumentNullException>(() => StringExtensions.Extract(null!, "1", "2"));
        Assert.Throws<ArgumentNullException>(() => "0".Extract(null!, "2"));
        Assert.Throws<ArgumentNullException>(() => "0".Extract("1", null!));

        string theExtract = "123abxyzcd789".Extract(between: "ab", and: "cd");
        Assert.That(theExtract, Is.EqualTo("xyz"));

        theExtract = "Hallo [Welt]!".Extract(between: "[", and: "]");
        Assert.That(theExtract, Is.EqualTo("Welt"));

        // Extracts the *first* occurence between "ab" and "cd":
        theExtract = "Hallo [Welt]! Dies ist [ein] Test.".Extract(between: "[", and: "]");
        Assert.That(theExtract, Is.EqualTo("Welt"));

        theExtract = "Hello".Extract(between: "(", and: ")");
        Assert.That(theExtract, Is.Empty);

        theExtract = "(Hello".Extract(between: "(", and: ")");
        Assert.That(theExtract, Is.EqualTo("Hello"));

        theExtract = ")Hello(".Extract(between: "(", and: ")");
        Assert.That(theExtract, Is.Empty);
    }

    [Test]
    public void Matches() {
        Assert.Throws<ArgumentNullException>(() => StringExtensions.Matches(null!, "1"));
        Assert.Throws<ArgumentNullException>(() => "0".Matches(null!));

        Assert.That("1.1.1".Matches(@"\d+\.\d+\.\d+"), Is.True);
        Assert.That("99.01.16".Matches(@"\d+\.\d+\.\d+"), Is.True);
        Assert.That("X.01.16".Matches(@"\d+\.\d+\.\d+"), Is.False);
    }

    [Test]
    public void IsValidEmailAddress() {
        Assert.Throws<ArgumentNullException>(() => StringExtensions.IsValidEmailAddress(null!));

        Assert.That("jan.bohlen@outlook.de".IsValidEmailAddress, Is.True);
        Assert.That("jan.bohlen@outlook".IsValidEmailAddress, Is.False);
        Assert.That("jan.bohlen".IsValidEmailAddress, Is.False);
        Assert.That("@outlook.de".IsValidEmailAddress, Is.False);
    }

    [Test]
    public void TryConvert()
    {
        bool isSuccess = "33".TryConvert(typeof(int), CultureInfo.InvariantCulture, out object? theObject);
        Assert.That(theObject, Is.EqualTo(33));
        Assert.That(isSuccess, Is.True);

        isSuccess = "XY".TryConvert(typeof(int), CultureInfo.InvariantCulture, out theObject);
        Assert.That(theObject, Is.Null);
        Assert.That(isSuccess, Is.False);

        isSuccess = "XY".TryConvert(typeof(StringExtensions), CultureInfo.InvariantCulture, out theObject);
        Assert.That(theObject, Is.Null);
        Assert.That(isSuccess, Is.False);

        isSuccess = "33".TryConvert(CultureInfo.InvariantCulture, out int theInteger);
        Assert.That(theInteger, Is.EqualTo(33));
        Assert.That(isSuccess, Is.True);

        isSuccess = "ABC".TryConvert(CultureInfo.InvariantCulture, out double theDouble);
        Assert.That(theDouble, Is.EqualTo(0.0));
        Assert.That(isSuccess, Is.False);
    }
}
