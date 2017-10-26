using System;
using System.Collections.Generic;

namespace Epos.Utilities
{
    /// <summary>Collection of extension methods for the
    /// <see cref="System.Collections.Generic.IList{T}" /> type.</summary>
    public static class ListExtensions
    {
        /// <summary>Compares two <see cref="System.Collections.Generic.IList{T}"/> instances.
        /// </summary>
        /// <remarks>The element count, the elements and their order must match.</remarks>
        /// <param name="list">List</param>
        /// <param name="anotherList">List</param>
        /// <returns><b>true</b>, if both lists are equal; otherwise <b>false</b></returns>
        public static bool EqualsList<T>(this IList<T> list, IList<T> anotherList) {
            if (list == null) {
                throw new ArgumentNullException(nameof(list));
            }
            if (anotherList == null) {
                throw new ArgumentNullException(nameof(anotherList));
            }

            if (ReferenceEquals(list, anotherList)) {
                return true;
            }

            if (list.Count != anotherList.Count) {
                return false;
            }

            int theCount = list.Count;
            IEqualityComparer<T> theEqualityComparer = EqualityComparer<T>.Default;

            for (int theIndex = 0; theIndex < theCount; theIndex++) {
                if (!theEqualityComparer.Equals(list[theIndex], anotherList[theIndex])) {
                    return false;
                }
            }

            return true;
        }

        /// <summary>Calculates a combined hashcode of all entries in the specified
        /// <see cref="System.Collections.Generic.IList{T}"/>. </summary>
        /// <param name="list">List</param>
        /// <returns>List hashcode</returns>
        public static int GetListHashCode<T>(this IList<T> list) {
            if (list == null) {
                throw new ArgumentNullException(nameof(list));
            }

            int theResult = 12345;
            foreach (T theObject in list) {
                theResult =
                    37 * theResult +
                    (!ReferenceEquals(theObject, null) ? theObject.GetHashCode() : 0);
            }

            return theResult;
        }
    }
}
