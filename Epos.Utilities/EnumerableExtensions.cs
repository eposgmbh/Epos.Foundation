using System;
using System.Collections.Generic;

namespace Epos.Utilities
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> ToEnumerable<T>(this T value) {
            yield return value;
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action) {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }
            if (action == null) {
                throw new ArgumentNullException(nameof(action));
            }

            foreach (T theObject in source) {
                action(theObject);
            }
        }

        public static IEnumerable<T> Traverse<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> recurseFunc) {
            foreach (T theItem in source) {
                yield return theItem;

                IEnumerable<T> theRecursiveEnumerable = recurseFunc(theItem);
                if (theRecursiveEnumerable != null) {
                    foreach (T theRecursiveItem in Traverse(theRecursiveEnumerable, recurseFunc)) {
                        yield return theRecursiveItem;
                    }
                }
            }
        }

        public static void DisposeAll(this IEnumerable<IDisposable> source) {
            source.ForEach(d => d.Dispose());
        }
    }
}