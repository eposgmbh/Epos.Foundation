namespace Epos.Utilities;

/// <summary>Provides a general in-memory Cache.</summary>
public interface ICache
{
    /// <summary> Deactivates the caching for newly created instances (via
    /// <see cref="ICache{TKey, TValue}.Create"/>), if set to true </summary>
    public static bool DeactivateCachingForNewInstances { get; set; }

    /// <summary>Gets the cache capacity. </summary>
    /// <returns>Cache capacity</returns>
    public int Capacity { get; }

    /// <summary>Gets the value count of the cache.</summary>
    /// <returns>Cache value count</returns>
    public int Count { get; }

    /// <summary>Returns a string that represents the cache.</summary>
    /// <returns>String representation of the cache</returns>
    public string ToString();
}

/// <summary>Provides a general in-memory Cache.</summary>
/// <typeparam name="TKey">Cache key type</typeparam>
/// <typeparam name="TValue">Cache value type (must be a <b>class</b>)</typeparam>
public interface ICache<TKey, TValue> : ICache
    where TKey : notnull
    where TValue : class
{
    /// <summary> Creates a new <seealso cref="ICache{TKey, TValue}"/> instance. </summary>
    /// <param name="capacity">Cache capacity</param>
    /// <returns><seealso cref="ICache{TKey, TValue}"/> instance</returns>
    public static ICache<TKey, TValue> Create(int capacity) =>
        DeactivateCachingForNewInstances
            ? new NullCache<TKey, TValue>()
            : new DefaultCache<TKey, TValue>(capacity);

    /// <summary>Sets or returns a cache value.</summary>
    /// <param name="key">Cache key</param>
    /// <returns>Value or <b>null</b>, if no value is found</returns>
    public TValue? this[TKey key] { get; }

    /// <param name="key">Key</param>
    /// <param name="value">Value</param>
    public void Add(TKey key, TValue value);
}
