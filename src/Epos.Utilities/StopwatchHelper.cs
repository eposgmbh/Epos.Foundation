using System;
using System.Diagnostics;

namespace Epos.Utilities;

/// <summary>Helper for measuring an action's duration in milliseconds.
/// </summary>
public static class StopwatchHelper
{
    /// <summary>Gets the number of milliseconds the specified
    /// <see cref="System.Action"/> takes.</summary>
    /// <param name="action">Action</param>
    /// <returns>Number of milliseconds</returns>
    public static long GetMilliseconds(Action action) {
        if (action is null) {
            throw new ArgumentNullException(nameof(action));
        }

        Stopwatch theStopwatch = Stopwatch.StartNew();

        action();

        return theStopwatch.ElapsedMilliseconds;
    }
}
