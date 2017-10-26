using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Epos.Utilities
{
    /// <summary>Provides a general in-memory Cache.</summary>
    /// <typeparam name="TKey">Cache key type</typeparam>
    /// <typeparam name="TValue">Cache value type (must be a <b>class</b>)</typeparam>
    public sealed class Cache<TKey, TValue> where TValue : class
    {
        private const int ConcurrencyLevel = 4;
        private readonly ConcurrentDictionary<TKey, TValue> myInternalCache;
        private readonly List<TKey> myQueue;

        /// <summary>Initializes an instance of the <see cref="Cache{TKey,TValue}"/> class.</summary>
        /// <param name="capacity">Cache capacity</param>
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
		
        /// <summary>Sets or returns a cache value.</summary>
        /// <param name="key">Cache key</param>
        /// <returns>Value or <b>null</b>, if no value is found</returns>
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

        /// <summary>Gets the cache capacity. </summary>
        /// <returns>Cache capacity</returns>
        public int Capacity { get; }

        /// <summary>Gets the value count of the cache.</summary>
        /// <returns>Cache value count</returns>
        public int Count => myQueue.Count;

        /// <summary>Returns a string that represents the cache.</summary>
        /// <returns>String representation of the cache</returns>
        public override string ToString() {
            IEnumerable theDumpFriendlyEnumerable =
                from theKey in myQueue
                let theValue = this[theKey]
                select new { Key = theKey, Value = theValue };

            return theDumpFriendlyEnumerable.Dump();
        }
    }

    // DEV Hint: Implementation mit Weak-References lässt sich noch unter Changeset 94 finden.
}
