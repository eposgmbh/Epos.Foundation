using System;
using System.Collections.Generic;

namespace Epos.Utilities
{
    /// <summary>Collection of extension methods for the
    /// <see cref="System.Collections.Generic.IEnumerable{T}" /> type.</summary>
    public static class EnumerableExtensions
    {
        /// <summary>Converts a single value into an <see cref="System.Collections.Generic.IEnumerable{T}"/>
        /// instance.</summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="value">Value</param>
        /// <returns><see cref="System.Collections.Generic.IEnumerable{T}"/> instance</returns>
        public static IEnumerable<T> ToEnumerable<T>(this T value) {
            yield return value;
        }

        /// <summary> Calls an <paramref name="action"/> for each element of the
        /// specified <see cref="System.Collections.Generic.IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="source">Sequence</param>
        /// <param name="action">Action</param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action) {
            DebugThrow.IfNull(source, nameof(source));
            DebugThrow.IfNull(action, nameof(action));

            foreach (T theObject in source) {
                action(theObject);
            }
        }

        /// <summary>Flattens a recursive hierarchy of the specified <paramref name="source"/>.
        /// The <paramref name="recurseFunc"/> is used to access the children of the current
        /// element.
        /// </summary>
        /// <example><code><![CDATA[
        /// var theRoot = new Child("Father") {
        ///     Children = {
        ///         new Child("Son") {
        ///             Children = {
        ///                 new Child("Grandson 1"),
        ///                 new Child("Grandson 2")
        ///             }
        ///         }
        ///     }
        /// };
        ///
        /// var theFlattenedSeq = theRoot.ToEnumerable().FlattenRecursiveHierarchy(c => c.Children);
        /// ]]></code></example>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="source">Sequence</param>
        /// <param name="recurseFunc">Used to access the children of the current element</param>
        /// <returns>Flattened sequence</returns>
        public static IEnumerable<T> FlattenRecursiveHierarchy<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> recurseFunc) {
            DebugThrow.IfNull(source, nameof(source));
            DebugThrow.IfNull(recurseFunc, nameof(recurseFunc));

            foreach (T theItem in source) {
                yield return theItem;

                IEnumerable<T> theRecursiveEnumerable = recurseFunc(theItem);
                if (theRecursiveEnumerable is not null) {
                    foreach (T theRecursiveItem in FlattenRecursiveHierarchy(theRecursiveEnumerable, recurseFunc)) {
                        yield return theRecursiveItem;
                    }
                }
            }
        }
    }
}
