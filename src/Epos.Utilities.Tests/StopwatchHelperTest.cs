using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Epos.Utilities
{
    [TestFixture]
    public class StopwatchHelperTest
    {
        [Test]
        public void GetMilliseconds() {
            Assert.Throws<ArgumentNullException>(() => StopwatchHelper.GetMilliseconds(null!));

            long theMilliseconds = StopwatchHelper.GetMilliseconds(() => {
                Task.Delay(100).Wait();
            });

            Assert.That(theMilliseconds, Is.GreaterThan(0L));
        }
    }
}
