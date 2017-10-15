using System;

namespace Epos.Utilities.Composition
{
    public sealed class ImplementedByOptions<TAbstract, TConcrete> : LifetimeOptions where TConcrete : TAbstract
    {
        internal ImplementedByOptions(ComponentRegistration componentRegistration) : base(componentRegistration) {
            ComponentRegistration.ConcreteType = typeof (TConcrete);
        }

        public WithParameterOptions WithParameter(string parameterName, object value) {
            if (parameterName == null) {
                throw new ArgumentNullException(nameof(parameterName));
            }

            return new WithParameterOptions(parameterName, value, ComponentRegistration);
        }
    }
}