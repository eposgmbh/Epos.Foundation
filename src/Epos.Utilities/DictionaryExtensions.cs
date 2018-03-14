using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Epos.Utilities
{
    /// <summary>Collection of extension methods for the
    /// <see cref="System.Collections.Generic.IDictionary{TKey,TValue}" /> type.</summary>
    public static class DictionaryExtensions
    {
        /// <summary>Gets the value for a specified key or <b>null</b>, if no
        /// value is found.</summary>
        /// <typeparam name="TKey">Key type</typeparam>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="dictionary">Dictionary</param>
        /// <param name="key">Key</param>
        /// <returns>Value or <b>null</b>, if no value is found</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
            where TValue : class {
            DebugThrow.IfNull(dictionary, nameof(dictionary));

            dictionary.TryGetValue(key, out TValue theValue);
            return theValue;
        }
    }
}
