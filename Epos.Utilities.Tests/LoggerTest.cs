#if  DEBUG // Tests machen nur in DEBUG Sinn, da Logger-Aufrufe im Release-Build entfernt werden.

using System;
using System.Text;

using NUnit.Framework;

namespace Epos.Utilities
{
    [TestFixture]
    public class LoggerTest
    {
        [Test]
        public void SetLogAction() {
            bool isLoggerActionCalled = false;
            Logger.SetLogAction(str => { isLoggerActionCalled = true; });
            Assert.That(isLoggerActionCalled, Is.False);
            
            Logger.Log("Hello World!");
            Assert.That(isLoggerActionCalled, Is.True);

            Assert.Throws<ArgumentNullException>(() => Logger.SetLogAction(null));
        }

        [Test]
        public void Log() {
            StringBuilder theBuilder = new StringBuilder();
            Logger.SetLogAction(s => theBuilder.Append(s));

            Logger.Log("This ");
            Logger.Log("is ");
            Logger.Log("a ");
            Logger.Log("test.");

            Assert.That(theBuilder.ToString(), Is.EqualTo("This is a test."));
        }

        [Test]
        public void LogLine() {
            StringBuilder theBuilder = new StringBuilder();
            Logger.SetLogAction(s => theBuilder.Append(s));

            Logger.LogLine("This");
            Logger.LogLine("is");
            Logger.LogLine("a");
            Logger.LogLine("test.");

            Assert.That(theBuilder.ToString(), Is.EqualTo(
                "This" + Environment.NewLine +
                "is" + Environment.NewLine +
                "a" + Environment.NewLine +
                "test." + Environment.NewLine)
            );
        }
    }
}

#endif
