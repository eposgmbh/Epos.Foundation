namespace Epos.Utilities;

/// <inheritdoc />
public sealed class NullCache<TKey, TValue> : ICache<TKey, TValue>
    where TKey : notnull
    where TValue : class
{
    /// <inheritdoc />
    public TValue? this[TKey key] => null;

    /// <inheritdoc />
    public void Add(TKey key, TValue value) { }

    /// <inheritdoc />
    public int Capacity { get; }

    /// <inheritdoc />
    public int Count => 0;

    /// <inheritdoc />
    public override string ToString() => "[]";
}
