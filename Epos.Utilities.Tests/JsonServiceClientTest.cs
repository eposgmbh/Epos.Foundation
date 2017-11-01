using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Epos.Utilities
{
    [TestFixture]
    public class JsonServiceClientTest
    {
        [Test]
        public void Constructor() {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new JsonServiceClient(null));

            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentException>(() => new JsonServiceClient(string.Empty));

            var theClient = new JsonServiceClient("   http://url.web.api    ");
            Assert.That(theClient.RootUrl, Is.EqualTo("http://url.web.api/"));
        }

        [Test]
        public async Task GetOne() {
            var theClient = new JsonServiceClient("https://postman-echo.com/");

            var theGetOneResult = await theClient.GetOneAsync<GetOneResult>(
                "time/format", ("timestamp", "2016-10-10"), ("format", "mm")
            );

            Assert.That(theGetOneResult.Format, Is.EqualTo(20));
        }

        [Test]
        public async Task GetMany() {
            var theClient = new JsonServiceClient("https://jsonplaceholder.typicode.com/");

            var theGetManyResult = await theClient.GetManyAsync<GetManyResult>(
                "posts", ("userId", 1)
            );

            Assert.That(theGetManyResult, Has.Count.EqualTo(10));

            var theFirstResult = theGetManyResult.First();
            Assert.That(theFirstResult.UserId, Is.EqualTo(1));
            Assert.That(theFirstResult.Id, Is.EqualTo(1));
            Assert.That(theFirstResult.Title, Is.EqualTo(
                "sunt aut facere repellat provident occaecati excepturi optio reprehenderit"
            ));
            Assert.That(theFirstResult.Body, Is.EqualTo(
                "quia et suscipit\nsuscipit recusandae consequuntur expedita et cum\n" +
                "reprehenderit molestiae ut ut quas totam\nnostrum rerum est autem sunt " +
                "rem eveniet architecto"
            ));
        }

        [Test]
        public async Task Post() {
            var theClient = new JsonServiceClient("https://jsonplaceholder.typicode.com/");

            var thePostResult = await theClient.PostAsync<PostResult>(
                "posts",
                new PostMessage {
                    UserId = 1,
                    Title = "foo",
                    Body = "bar"
                }
            );

            Assert.That(thePostResult.UserId, Is.EqualTo(1));
            Assert.That(thePostResult.Id, Is.EqualTo(101));
            Assert.That(thePostResult.Title, Is.EqualTo("foo"));
            Assert.That(thePostResult.Body, Is.EqualTo("bar"));
        }

        #region Helper classes

        // ReSharper disable once ClassNeverInstantiated.Local
        private class GetOneResult
        {
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public int Format { get; set; }
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class GetManyResult
        {
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public int UserId { get; set; }

            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public int Id { get; set; }

            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public string Title { get; set; }

            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public string Body { get; set; }
        }

        private class PostMessage : GetManyResult { }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class PostResult : GetManyResult { }

        #endregion
    }
}
