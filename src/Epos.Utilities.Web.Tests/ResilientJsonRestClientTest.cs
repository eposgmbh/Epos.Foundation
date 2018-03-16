using System;
using NUnit.Framework;

namespace Epos.Utilities.Web
{
    public class ResilientJsonRestClientTest : JsonServiceClientTestBase
    {
        protected override IJsonRestClient CreateClient(string rootUri) =>
            new ResilientJsonRestClient(new PolicyProvider(), rootUri);

        [Test]
        public void Constructor() {
            Assert.Throws<ArgumentNullException>(() => new ResilientJsonRestClient(null, null));
            Assert.Throws<ArgumentNullException>(() => new ResilientJsonRestClient(new PolicyProvider(), null));
            Assert.Throws<ArgumentNullException>(() => new ResilientJsonRestClient(null, string.Empty));

            Assert.Throws<ArgumentException>(() => new ResilientJsonRestClient(new PolicyProvider(), string.Empty));

            var theClient = new JsonRestClient("   http://url.web.api    ");
            Assert.That(theClient.RootUri, Is.EqualTo("http://url.web.api/"));
        }
    }
}
