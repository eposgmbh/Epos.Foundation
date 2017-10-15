using System;
using NUnit.Framework;

namespace Epos.Utilities
{
    [TestFixture]
    public class PasswordTest
    {
        [Test]
        public void Basics() {
            Assert.Throws<ArgumentNullException>(() => Password.Encrypt(null));
            Assert.Throws<ArgumentNullException>(() => Password.Decrypt(null));

            string theBase64 = Password.Encrypt("Hallo, Welt!");

            Assert.AreEqual("BFzXiq9YSIvSl+fKtxrPhw==", theBase64);

            string theHalloWelt = Password.Decrypt(theBase64);

            Assert.AreEqual("Hallo, Welt!", theHalloWelt);
        }
    }
}
