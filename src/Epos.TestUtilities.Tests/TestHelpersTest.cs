using System;
using System.IO;
using NUnit.Framework;

namespace Epos.TestUtilities;

[TestFixture]
public class TestHelpersTest
{
    [Test]
    public void ProjectFolder() {
        string projectFolder = TestHelpers.ProjectFolder;

        Assert.That(projectFolder, Is.Not.Null);
        Assert.That(projectFolder, Does.EndWith("Epos.TestUtilities.Tests"));
    }

    [Test]
    public void GetResourceStream() {
        Assert.Throws<ArgumentException>(() => TestHelpers.GetResourceStream(null!));
        Assert.Throws<ArgumentException>(() => TestHelpers.GetResourceStream("InvalidResourceName"));

        using Stream stream = TestHelpers.GetResourceStream("Epos.TestUtilities.Resources.TestResource.txt");

        Assert.That(stream, Is.Not.Null);
        Assert.That(stream.Length, Is.GreaterThan(0));
    }

    [Test]
    public void GetResourceString() {
        string content = TestHelpers.GetResourceString("Epos.TestUtilities.Resources.TestResource.txt");

        Assert.That(content, Is.Not.Null);
        Assert.That(content, Is.EqualTo("Hello, World!"));
    }
}
