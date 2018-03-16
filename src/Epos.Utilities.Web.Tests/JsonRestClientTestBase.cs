using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Epos.Utilities.Web
{
    [TestFixture]
    public abstract class JsonServiceClientTestBase
    {
        protected abstract IJsonRestClient CreateClient(string rootUri);

        [Test]
        public async Task GetOne() {
            var theClient = CreateClient("https://postman-echo.com/");

            var theGetOneResult = await theClient.GetAsync<GetOneResult>(
                "time/format", ("timestamp", "2016-10-10"), ("format", "mm")
            );

            Assert.That(theGetOneResult.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(theGetOneResult.Response.Format, Is.EqualTo(20));
        }

        [Test]
        public async Task GetMany() {
            var theClient = CreateClient("https://jsonplaceholder.typicode.com/");

            var theGetManyResult = await theClient.GetManyAsync<GetManyResult>(
                apiUri: "posts", queryParams: ("userId", 1)
            );

            Assert.That(theGetManyResult.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(theGetManyResult.Response, Has.Count.GreaterThanOrEqualTo(10));

            var theFirstResult = theGetManyResult.Response.First();
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
            var theClient = CreateClient("https://jsonplaceholder.typicode.com/");

            var (theStatusCode, _) = await theClient.PostAsync(
                "posts",
                new PostAndPutMessage {
                    UserId = 1,
                    Title = "foo",
                    Body = "bar"
                }
            );

            Assert.That(theStatusCode, Is.EqualTo(HttpStatusCode.Created));
        }

        [Test]
        public async Task Put() {
            var theClient = CreateClient("https://jsonplaceholder.typicode.com/");

            var (theStatusCode, _) = await theClient.PutAsync(
                "posts/1",
                new PostAndPutMessage {
                    Id = 1,
                    UserId = 1,
                    Title = "foo",
                    Body = "bar"
                }
            );

            Assert.That(theStatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Delete() {
            var theClient = CreateClient("https://jsonplaceholder.typicode.com/");

            var (theStatusCode, _) = await theClient.DeleteAsync("posts/1");

            Assert.That(theStatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        #region Helper classes

        private class GetOneResult
        {
            public int Format { get; set; }
        }

        private class GetManyResult
        {
            public int UserId { get; set; }
            public int Id { get; set; }
            public string Title { get; set; }
            public string Body { get; set; }
        }

        private class PostAndPutMessage : GetManyResult { }

        #endregion
    }
}
