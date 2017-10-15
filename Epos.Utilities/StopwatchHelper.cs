using System;
using System.Diagnostics;

namespace Epos.Utilities
{
    public static class StopwatchHelper
    {
        public static long GetMilliseconds(Action action) {
            if (action == null) {
                throw new ArgumentNullException(nameof(action));
            }

            Stopwatch theStopwatch = Stopwatch.StartNew();

            action();

            return theStopwatch.ElapsedMilliseconds;
        }
    }
}