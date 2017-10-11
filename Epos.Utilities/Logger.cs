using System;
using System.Diagnostics;

namespace Epos.Utilities
{
    public static class Logger
    {
        public static readonly Action<string> ConsoleOutputAction = Console.Write;

        private static readonly object SyncLock = new object();

        [ThreadStatic]
        private static Action<string> myLogAction;

        [Conditional("LOGGING"), Conditional("CODE_ANALYSIS")]
        public static void SetLogAction(Action<string> logAction) {
            if (logAction == null) {
                throw new ArgumentNullException(nameof(logAction));
            }

            lock (SyncLock) {
                myLogAction = logAction;
            }
        }

        [Conditional("LOGGING"), Conditional("CODE_ANALYSIS")]
        public static void Log(string message) {
            if (myLogAction != null) {
                lock (SyncLock) {
                    myLogAction(message);
                }
            }
        }

        [Conditional("LOGGING"), Conditional("CODE_ANALYSIS")]
        public static void LogLine(string message) {
            if (myLogAction != null) {
                lock (SyncLock) {
                    myLogAction(message);
                    myLogAction(Environment.NewLine);
                }
            }
        }
    }
}