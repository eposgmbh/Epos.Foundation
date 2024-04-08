using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Epos.Utilities;

/// <summary>Provides a general in-memory Cache.</summary>
/// <typeparam name="TKey">Cache key type</typeparam>
/// <typeparam name="TValue">Cache value type (must be a <b>class</b>)</typeparam>
public sealed class Cache<TKey, TValue>
    where TKey : notnull
    where TValue : class
{
    private static readonly object SyncLock = new();

    private readonly Dictionary<TKey, TValue> myInternalCache;
    private readonly Queue<TKey> myQueue;

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
        myInternalCache = new Dictionary<TKey, TValue>(capacity + 1);
        myQueue = new Queue<TKey>(capacity + 1);
    }

    /// <summary>Sets or returns a cache value.</summary>
    /// <param name="key">Cache key</param>
    /// <returns>Value or <b>null</b>, if no value is found</returns>
    public TValue? this[TKey key] {
        get {
            myInternalCache.TryGetValue(key, out TValue? theResult);
            return theResult;
        }
    }

    /// <summary> Adds a cache value. </summary>
    /// <param name="key">Key</param>
    /// <param name="value">Value</param>
    public void Add(TKey key, TValue value) {
        lock (SyncLock) {
            if (!myInternalCache.ContainsKey(key)) {
                myQueue.Enqueue(key);
            }
            myInternalCache[key] = value;

            if (myQueue.Count > Capacity) {
                TKey theFirstKey = myQueue.Dequeue();
                myInternalCache.Remove(theFirstKey);
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
        lock (SyncLock) {
            IEnumerable theDumpFriendlyEnumerable =
                from theKey in myQueue
                let theValue = this[theKey]
                select new DictionaryEntry(theKey, theValue);

            return theDumpFriendlyEnumerable.Dump();
        }
    }
}
