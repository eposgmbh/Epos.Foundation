using System;
using System.ComponentModel;

using NUnit.Framework;

namespace Epos.Utilities
{
    [TestFixture]
    public class EventExtensionsTest
    {
        [Test]
        public void Raise1() {
            bool isFirstEventRaised = false;

            var theHelper = new EventHelper();
            theHelper.FirstEvent += (sender, e) => {
                Assert.That(sender, Is.EqualTo("Hello"));
                Assert.That(e, Is.SameAs(EventArgs.Empty));
                isFirstEventRaised = true;
            };

            theHelper.FirstEvent.Raise("Hello");

            Assert.That(isFirstEventRaised);
        }

        [Test]
        public void Raise2() {
            bool isFirstEventRaised = false;
            var theEventArgs = new AsyncCompletedEventArgs(null, false, null);

            var theHelper = new EventHelper();
            theHelper.FirstEvent += (sender, e) => {
                Assert.That(sender, Is.EqualTo("World"));
                Assert.That(e, Is.SameAs(theEventArgs));
                isFirstEventRaised = true;
            };

            theHelper.FirstEvent.Raise("World", theEventArgs);

            Assert.That(isFirstEventRaised);
        }

        [Test]
        public void Raise3() {
            bool isSecondEventRaised = false;
            var theEventArgs = new CancelEventArgs(false);

            var theHelper = new EventHelper();
            theHelper.SecondEvent += (sender, e) => {
                Assert.That(sender, Is.EqualTo(1234));
                Assert.That(e, Is.SameAs(theEventArgs));
                isSecondEventRaised = true;
            };

            theHelper.SecondEvent.Raise(1234, theEventArgs);

            Assert.That(isSecondEventRaised);
        }

        [Test]
        public void Raise4() {
            bool isThirdEventRaised = false;
            
            var theHelper = new EventHelper();
            theHelper.ThirdEvent += (sender, e) => {
                Assert.That(sender, Is.EqualTo(null));
                Assert.That(e.PropertyName, Is.EqualTo("PropertyName"));
                isThirdEventRaised = true;
            };

            theHelper.ThirdEvent.Raise(null!, "PropertyName");

            Assert.That(isThirdEventRaised);
        }

        private class EventHelper
        {
            public EventHandler? FirstEvent;

            public EventHandler<CancelEventArgs>? SecondEvent;

            public PropertyChangedEventHandler? ThirdEvent;
        }
    }
}
