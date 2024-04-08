namespace Epos.Utilities.Composition;

/// <summary> See <see cref="LifetimeOptions.WithLifetime(Lifetime)"/>.
/// </summary>
public enum Lifetime
{
    /// <summary>Singleton</summary>
    Singleton,

    /// <summary>Transient</summary>
    Transient
}
