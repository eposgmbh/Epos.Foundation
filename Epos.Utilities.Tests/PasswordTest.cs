using System;
using NUnit.Framework;

namespace Epos.Utilities
{
    [TestFixture]
    public class PasswordTest
    {
        [Test]
        public void Hash() {
            Assert.Throws<ArgumentNullException>(() => Password.Hash(null));

            string theHash = Password.Hash("abcd1234");
            Assert.AreEqual("e9cee71ab932fde863338d08be4de9dfe39ea049bdafb342ce659ec5450b69ae", theHash);
        }
    }
}
