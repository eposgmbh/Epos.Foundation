using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Epos.Utilities
{
    public static class DebugThrow
    {
        [Conditional("DEBUG")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Global
        public static void IfNull<T>(T value, string paramName) where T : class {
            if (value == null) {
                throw new ArgumentNullException(paramName);
            }
        }

        [Conditional("DEBUG")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IfInvalidEnum(Enum value, string paramName) {
            if (!Enum.IsDefined(value.GetType(), value)) {
                throw new InvalidEnumArgumentException(paramName, (int) (object) value, value.GetType());
            }
        }

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
