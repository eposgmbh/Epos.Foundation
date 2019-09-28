using System;
using System.Diagnostics;

namespace Epos.Utilities
{
    /// <summary> Provides a high performance thread-safe logger implementation.
    /// </summary>
    /// <remarks> Calls to methods of this class are only compiled, if the compilation
    /// symbol <b>LOGGING</b> is defined. </remarks>
    public static class HighPerfLogger
    {
        private static readonly object SyncLock = new object();

        [ThreadStatic]
        private static Action<string>? myLogAction;

        /// <summary>Sets the log action on the current thread.</summary>
        /// <remarks>Calls to this method are only compiled, if the compilation
        /// symbol <b>LOGGING</b> is defined. This method is thread-safe.</remarks>
        /// <param name="logAction">Log action, for example <see cref="Console.Write(object)"/>
        /// </param>
        [Conditional("LOGGING")]
        public static void SetLogAction(Action<string> logAction) {
            if (logAction == null) {
                throw new ArgumentNullException(nameof(logAction));
            }

            lock (SyncLock) {
                myLogAction = logAction;
            }
        }

        /// <summary>Logs the specified <paramref name="message"/>.</summary>
        /// <remarks>Calls to this method are only compiled, if the compilation
        /// symbol <b>LOGGING</b> is defined. This method is thread-safe.</remarks>
        /// <param name="message">Log message</param>
        [Conditional("LOGGING")]
        public static void Log(string message) {
            if (myLogAction != null) {
                lock (SyncLock) {
                    myLogAction(message);
                }
            }
        }

        /// <summary>Logs the specified <paramref name="message"/> and appends
        /// <see cref="System.Environment.NewLine"/>.</summary>
        /// <remarks>Calls to this method are only compiled, if the compilation
        /// symbol <b>LOGGING</b> is defined. This method is thread-safe.</remarks>
        /// <param name="message">Log message</param>
        [Conditional("LOGGING")]
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
