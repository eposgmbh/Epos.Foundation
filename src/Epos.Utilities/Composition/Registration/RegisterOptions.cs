using System;

namespace Epos.Utilities.Composition
{
    /// <summary>Options following the <see cref="Container.Register{T}"/> fluent
    /// interface step.
    /// <list type="bullet">
    /// <item><description><see cref="ImplementedBy{TConcrete}"/></description></item>
    /// <item><description><see cref="WithFactoryMethod{TConcrete}"/></description></item>
    /// </list>
    /// </summary>
    /// <typeparam name="TAbstraction">Abstraction</typeparam>
    public sealed class RegisterOptions<TAbstraction> : LifetimeOptions
    {
        internal RegisterOptions(ComponentRegistration componentRegistration) : base(componentRegistration) {
        }

        /// <summary>Specifies the implementation type of the registered container component.
        /// </summary>
        /// <typeparam name="TImplementation">Implementation</typeparam>
        /// <returns>Options following this fluent interface step</returns>
        public ImplementedByOptions<TAbstraction, TImplementation> ImplementedBy<TImplementation>() where TImplementation : TAbstraction {
            ComponentRegistration.Container.TestAlreadyResolved();
            return new ImplementedByOptions<TAbstraction, TImplementation>(ComponentRegistration);
        }

        /// <summary>Specifies the factory method that creates instances of the registered
        /// container component.</summary>
        /// <typeparam name="TImplementation">Implementation</typeparam>
        /// <param name="factoryMethod">Factory method</param>
        /// <returns></returns>
        public WithFactoryMethodOptions<TAbstraction, TImplementation> WithFactoryMethod<TImplementation>(Func<TImplementation> factoryMethod)
            where TImplementation : TAbstraction {
            ComponentRegistration.Container.TestAlreadyResolved();
            return new WithFactoryMethodOptions<TAbstraction, TImplementation>(ComponentRegistration, factoryMethod);
        }
    }
}
