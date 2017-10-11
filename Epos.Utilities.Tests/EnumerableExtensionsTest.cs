using System;
using System.Text;

using NUnit.Framework;

using System.Linq;

namespace Epos.Utilities
{
    [TestFixture]
    public class EnumerableExtensionsTest
    {
        [Test]
        public void ToEnumerable() {
            var theInt32Enumerable = 1.ToEnumerable();
            Assert.That(theInt32Enumerable.Single(), Is.EqualTo(1));

            var theStringEnumerable = "Hello".ToEnumerable();
            Assert.That(theStringEnumerable.Single(), Is.EqualTo("Hello"));

            var theDateTimeEnumerable = new DateTime(2011, 04, 22).ToEnumerable();
            Assert.That(theDateTimeEnumerable.Single(), Is.EqualTo(new DateTime(2011, 04, 22)));

        }

        [Test]
        public void ForEach() {
            var theStringEnumerable = new[] { "This ", "is ", "a ", "test." };
            
            var theBuilder = new StringBuilder();
            theStringEnumerable.ForEach(s => theBuilder.Append(s));

            Assert.That(theBuilder.ToString(), Is.EqualTo("This is a test."));

            Assert.Throws<ArgumentNullException>(() => theStringEnumerable.ForEach(null));

            Assert.Throws<ArgumentNullException>(() => EnumerableExtensions.ForEach(null, (Action<object>) (obj => { })));
        }

        [Test]
        public void DisposeAll() {
            var theDisposables = new[] {
                new DisposableTestClass("This"),
                new DisposableTestClass("is"),
                new DisposableTestClass("a"),
                new DisposableTestClass("test")
            };

            foreach (DisposableTestClass theDisposable in theDisposables) {
                Assert.That(theDisposable.Result, Is.Null);
            }

            theDisposables.DisposeAll();

            Assert.That(theDisposables[0].Result, Is.EqualTo("This"));
            Assert.That(theDisposables[1].Result, Is.EqualTo("is"));
            Assert.That(theDisposables[2].Result, Is.EqualTo("a"));
            Assert.That(theDisposables[3].Result, Is.EqualTo("test"));
        }

        private sealed class DisposableTestClass : IDisposable
        {
            private readonly string myResult;
            private bool myIsDisposed;

            public DisposableTestClass(string result) {
                myResult = result;
            }

            public void Dispose() {
                myIsDisposed = true;
            }

            public string Result => myIsDisposed ? myResult : null;
        }
    }
}