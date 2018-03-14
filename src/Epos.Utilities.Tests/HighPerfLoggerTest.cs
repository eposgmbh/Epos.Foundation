#if  DEBUG // Tests machen nur in DEBUG Sinn, da HighPerfLogger-Aufrufe im Release-Build entfernt werden.

using System;
using System.Text;

using NUnit.Framework;

namespace Epos.Utilities
{
    [TestFixture]
    public class HighPerfLoggerTest
    {
        [Test]
        public void SetLogAction() {
            bool isLoggerActionCalled = false;
            HighPerfLogger.SetLogAction(str => { isLoggerActionCalled = true; });
            Assert.That(isLoggerActionCalled, Is.False);
            
            HighPerfLogger.Log("Hello World!");
            Assert.That(isLoggerActionCalled, Is.True);

            Assert.Throws<ArgumentNullException>(() => HighPerfLogger.SetLogAction(null));
        }

        [Test]
        public void Log() {
            StringBuilder theBuilder = new StringBuilder();
            HighPerfLogger.SetLogAction(s => theBuilder.Append(s));

            HighPerfLogger.Log("This ");
            HighPerfLogger.Log("is ");
            HighPerfLogger.Log("a ");
            HighPerfLogger.Log("test.");

            Assert.That(theBuilder.ToString(), Is.EqualTo("This is a test."));
        }

        [Test]
        public void LogLine() {
            StringBuilder theBuilder = new StringBuilder();
            HighPerfLogger.SetLogAction(s => theBuilder.Append(s));

            HighPerfLogger.LogLine("This");
            HighPerfLogger.LogLine("is");
            HighPerfLogger.LogLine("a");
            HighPerfLogger.LogLine("test.");

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
