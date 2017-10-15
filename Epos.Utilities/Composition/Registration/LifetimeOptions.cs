using System;

namespace Epos.Utilities.Composition
{
    public abstract class LifetimeOptions : RegistrationStep
    {
        internal LifetimeOptions(ComponentRegistration componentRegistration) : base(componentRegistration) {
        }

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

        private void WithLifetime(ComponentCreationStrategy strategy) {
            if (strategy == null) {
                throw new ArgumentNullException(nameof(strategy));
            }

            ComponentRegistration.Container.TestAlreadyResolved();
            ComponentRegistration.ComponentCreationStrategy = strategy;
        }
    }
}