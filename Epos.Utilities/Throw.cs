using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Epos.Utilities
{
    public static class Throw
    {
        [Conditional("DEBUG")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ArgumentNullException(object value, string paramName) {
            if (value == null) {
                throw new ArgumentNullException(paramName);
            }
        }
    }
}
