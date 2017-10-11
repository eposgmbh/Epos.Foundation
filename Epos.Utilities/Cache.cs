using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Epos.Utilities
{
    // Implementation mit Weak-References lässt sich noch unter Changeset 94 finden.

    /// <summary> Allgemeine Cache-Klasse </summary>
    /// <typeparam name="TKey">Typ des Cache-Schlüssels</typeparam>
    /// <typeparam name="TValue">Typ des Cache-Werts</typeparam>
    public sealed class Cache<TKey, TValue> where TValue : class
    {
        private const int ConcurrencyLevel = 4;
        private readonly ConcurrentDictionary<TKey, TValue> myInternalCache;
        private readonly List<TKey> myQueue;

        /// <summary> Erzeugt eine Instanz der Klasse <b>Cache</b>. </summary>
        /// <param name="capacity">Kapazität des Caches</param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="capacity"/> ist kleiner als 2.
        /// </exception>
        public Cache(int capacity) {
            if (capacity < 2) {
                throw new ArgumentOutOfRangeException(
                    nameof(capacity),
                    capacity,
                    "Capacity must be greater than one."
                );
            }

            Capacity = capacity;
            myInternalCache = new ConcurrentDictionary<TKey, TValue>(ConcurrencyLevel, capacity + 1);
            myQueue = new List<TKey>(capacity + 1);
        }
		
        /// <summary> Setzt oder liefert einen Wert aus dem Cache zurück. </summary>
        /// <param name="key">Schlüssel</param>
        /// <returns>Wert oder <b>null</b> (Nothing in VB), falls zu dem Schlüssel kein Wert existiert</returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="key"/> ist <b>null</b> (Nothing in VB).
        /// </exception>
        public TValue this[TKey key] {
            get {
                myInternalCache.TryGetValue(key, out TValue theResult);
                return theResult;
            }
            set {
                if (!myInternalCache.ContainsKey(key)) {
                    myQueue.Add(key);
                }
                myInternalCache[key] = value;

                if (myQueue.Count > Capacity) {
                    TKey theFirstKey = myQueue[0];
                    myQueue.RemoveAt(0);

                    myInternalCache.TryRemove(theFirstKey, out _);
                }
            }
        }

        /// <summary> Liefert die Kapazität des Caches zurück. </summary>
        public int Capacity { get; }

        /// <summary> Liefert die Anzahl der Elemente im Cache zurück. </summary>
        public int Count => myQueue.Count;

        /// <summary> Liefert eine String-Repräsentation des Caches zurück. </summary>
        /// <returns>String-Repräsentation des Caches</returns>
        public override string ToString() {
            IEnumerable theDumpFriendlyEnumerable =
                from theKey in myQueue
                let theValue = this[theKey]
                select new { Key = theKey, Value = theValue };

            return theDumpFriendlyEnumerable.Dump();
        }
    }
}
