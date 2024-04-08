using System;

namespace Epos.Utilities.Composition;

/// <summary>Lifetime options following several fluent interface steps.
/// <list type="bullet">
/// <item><description><see cref="WithLifetime(ComponentCreationStrategy)"/></description></item>
/// <item><description><see cref="WithLifetime(Lifetime)"/></description></item>
/// </list>
/// </summary>
public abstract class LifetimeOptions : RegistrationStep
{
    internal LifetimeOptions(ComponentRegistration componentRegistration) : base(componentRegistration) {
    }

    /// <summary>Specifies the lifetime of the implementation.
    /// </summary>
    /// <param name="lifetime">Lifetime</param>
    public void WithLifetime(Lifetime lifetime) {
        if (lifetime < Lifetime.Singleton || lifetime > Lifetime.Transient) {
            throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, "lifetime has an invalid enum value.");
        }

        if (lifetime == Lifetime.Singleton) {
            WithLifetime(new SingletonComponentCreationStrategy());
        } else {
            WithLifetime(new TransientComponentCreationStrategy());
        }
    }

    /// <summary>Specifies a component creation strategy for the implementation.
    /// </summary>
    /// <remarks>You can implement your own component creation strategy like
    /// "Request" (a.k.a. "Scope") by deriving from <see cref="ComponentCreationStrategy"/>.
    /// </remarks>
    /// <param name="strategy">Component creation strategy</param>
    public void WithLifetime(ComponentCreationStrategy strategy) {
        ComponentRegistration.Container.TestAlreadyResolved();
        ComponentRegistration.ComponentCreationStrategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
    }
}
