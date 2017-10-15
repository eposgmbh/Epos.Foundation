using System;

namespace Epos.Utilities.Composition
{
    public sealed class RegisterOptions<TAbstract> : LifetimeOptions
    {
        internal RegisterOptions(ComponentRegistration componentRegistration) : base(componentRegistration) {
        }

        public ImplementedByOptions<TAbstract, TConcrete> ImplementedBy<TConcrete>() where TConcrete : TAbstract {
            ComponentRegistration.Container.TestAlreadyResolved();
            return new ImplementedByOptions<TAbstract, TConcrete>(ComponentRegistration);
        }

        public WithFactoryMethodOptions<TAbstract, TConcrete> WithFactoryMethod<TConcrete>(Func<TConcrete> factoryMethod)
            where TConcrete : TAbstract {
            ComponentRegistration.Container.TestAlreadyResolved();
            return new WithFactoryMethodOptions<TAbstract, TConcrete>(ComponentRegistration, factoryMethod);
        }
    }
}