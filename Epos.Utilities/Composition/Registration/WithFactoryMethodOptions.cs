using System;

namespace Epos.Utilities.Composition
{
    public sealed class WithFactoryMethodOptions<TAbstract, TConcrete> : HasLifetime where TConcrete : TAbstract
    {
        internal WithFactoryMethodOptions(ComponentRegistration componentRegistration, Func<TConcrete> factoryMethod)
            : base(componentRegistration) {
            ComponentRegistration.FactoryMethod = factoryMethod;
        }
    }
}