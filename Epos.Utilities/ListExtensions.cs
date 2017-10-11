using System;
using System.Collections.Generic;

namespace Epos.Utilities
{
    /// <summary> Hilfsmethoden zum Umgang
    /// mit dem Datentyp <see cref="IList{T}"/>.
    /// </summary>
    public static class ListExtensions
    {
        /// <summary> Vergleicht zwei übergebene <see cref="IList{T}"/>. </summary>
        /// <remarks> Die Anzahl der Elemente sowie die einzelnen Elemente und ihre Reihenfolge müssen übereinstimmen.
        /// </remarks>
        /// <param name="list">Liste</param>
        /// <param name="anotherList">Liste</param>
        /// <returns><b>true</b>, falls die beiden Listen wertgleich sind; ansonsten <b>false</b></returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="list"/> ist <b>null</b> (Nothing in VB) -oder- <paramref name="anotherList"/> ist
        ///     <b>null</b> (Nothing in VB).
        /// </exception>
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

        /// <summary> Berechnet einen Hash-Code aus den Einträgen einer <see cref="IList{T}"/>. </summary>
        /// <param name="list">Liste</param>
        /// <returns>Hash-Code der Liste</returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="list"/> ist <b>null</b> (Nothing in VB).
        /// </exception>
        public static int GetListHashCode<T>(this IList<T> list) {
            if (list == null) {
                throw new ArgumentNullException(nameof(list));
            }

            int theResult = 0;
            foreach (T theObject in list) {
                theResult ^= !ReferenceEquals(theObject, null) ? theObject.GetHashCode() : 0;
            }

            return theResult;
        }
    }
}