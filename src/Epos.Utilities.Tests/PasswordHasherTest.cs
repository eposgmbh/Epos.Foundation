using System;
using System.Text;
using NUnit.Framework;

namespace Epos.Utilities;

[TestFixture]
public class PasswordHasherTest
{
    [Test]
    public void HashAndVerify() {
        var thePasswordHasher = new PasswordHasherSHA512();
        Assert.Throws<ArgumentNullException>(() => thePasswordHasher.Hash(null!));

        string theHashedPassword = thePasswordHasher.Hash("abcd1234");

        Assert.That(thePasswordHasher.Verify("abcd1234", theHashedPassword), Is.True);
        Assert.That(thePasswordHasher.Verify("1234abcd", theHashedPassword), Is.False);
    }
}
