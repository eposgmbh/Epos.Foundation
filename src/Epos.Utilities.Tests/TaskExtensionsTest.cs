using System.Threading.Tasks;
using NUnit.Framework;

namespace Epos.Utilities
{
    [TestFixture]
    public class TaskExtensionsTest
    {
        [Test]
        public void FireAndForget() {
            Task.Delay(10).FireAndForget();
        }
    }
}