using System;
using NUnit.Framework;

namespace Epos.Utilities.Web
{
    public class JsonRestClientTest : JsonServiceClientTestBase
    {
        protected override IJsonRestClient CreateClient(string rootUri) => new JsonRestClient(rootUri);

        [Test]
        public void Constructor() {
            Assert.Throws<ArgumentNullException>(() => new JsonRestClient(null));

            Assert.Throws<ArgumentException>(() => new JsonRestClient(string.Empty));

            var theClient = new JsonRestClient("   http://url.web.api    ");
            Assert.That(theClient.RootUri, Is.EqualTo("http://url.web.api/"));
        }
    }
}
