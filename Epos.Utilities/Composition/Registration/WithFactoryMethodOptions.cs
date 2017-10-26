using System;

namespace Epos.Utilities.Composition
{
    /// <summary> Options following the <see cref="RegisterOptions{TAbstraction}.WithFactoryMethod{TImplementation}"/>
    /// fluent interface step.</summary>
    /// <typeparam name="TAbstraction"></typeparam>
    /// <typeparam name="TImplementation"></typeparam>
    public sealed class WithFactoryMethodOptions<TAbstraction, TImplementation> : LifetimeOptions where TImplementation : TAbstraction
    {
        internal WithFactoryMethodOptions(ComponentRegistration componentRegistration, Func<TImplementation> factoryMethod)
            : base(componentRegistration) {
            ComponentRegistration.FactoryMethod = factoryMethod;
        }
    }
}
