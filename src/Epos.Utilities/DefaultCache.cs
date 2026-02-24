using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Epos.Utilities;

/// <inheritdoc />
public sealed class DefaultCache<TKey, TValue> : ICache<TKey, TValue>
    where TKey : notnull
    where TValue : class
{
    private static readonly Lock SyncLock = new();

    private readonly Dictionary<TKey, TValue> myInternalCache;
    private readonly Queue<TKey> myQueue;

    /// <summary>Initializes an instance of the <see cref="DefaultCache{TKey,TValue}"/> class.</summary>
    /// <param name="capacity">Cache capacity</param>
    public DefaultCache(int capacity) {
        if (capacity < 2) {
            throw new ArgumentOutOfRangeException(nameof(capacity), capacity, "Capacity must be greater than one.");
        }

        Capacity = capacity;
        myInternalCache = new Dictionary<TKey, TValue>(capacity + 1);
        myQueue = new Queue<TKey>(capacity + 1);
    }

    /// <inheritdoc />
    public TValue? this[TKey key] {
        get {
            myInternalCache.TryGetValue(key, out TValue? theResult);
            return theResult;
        }
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public int Capacity { get; }

    /// <inheritdoc />
    public int Count => myQueue.Count;

    /// <inheritdoc />
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
