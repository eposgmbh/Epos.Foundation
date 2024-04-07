using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Epos.Utilities
{
    /// <summary>Collection of utility methods for parameter assertions that
    /// will not be compiled into release builds for optimal performance.</summary>
    public static class DebugThrow
    {
        /// <summary>Throws an <see cref="ArgumentNullException"/>, if the specified
        /// <paramref name="value"/> is <b>null</b>.</summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <param name="value">Value to check for <b>null</b></param>
        /// <param name="paramName">Param name</param>
        [Conditional("DEBUG")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Global
        public static void IfNull<T>(T? value, string paramName) where T : class {
            if (value is null) {
                throw new ArgumentNullException(paramName);
            }
        }

        /// <summary>Throws an <b>InvalidEnumArgumentException</b>, if the
        /// specified <paramref name="value"/> is not a valid enum value.</summary>
        /// <param name="value">Value to check</param>
        /// <param name="paramName">Param name</param>
        [Conditional("DEBUG")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IfInvalidEnum(Enum value, string paramName) {
            if (!Enum.IsDefined(value.GetType(), value)) {
                throw new InvalidEnumArgumentException(paramName, (int) (object) value, value.GetType());
            }
        }

        /// <summary>Throws an <see cref="ArgumentException"/>, if the specified
        /// <paramref name="predicate"/> is <b>true</b>.</summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="message">Exception message</param>
        /// <param name="paramName">Param name</param>
        [Conditional("DEBUG")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Global
        public static void If(bool predicate, string message, string paramName) {
            if (predicate) {
                throw new ArgumentException(message, paramName);
            }
        }
    }
}
