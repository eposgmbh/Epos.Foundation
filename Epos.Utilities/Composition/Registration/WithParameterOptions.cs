using System;

namespace Epos.Utilities.Composition
{
    public sealed class WithParameterOptions : HasLifetime
    {
        internal WithParameterOptions(string parameterName, object value, ComponentRegistration componentRegistration)
            : base(componentRegistration) {
            ComponentRegistration.Parameters[parameterName] = value;
        }

        public WithParameterOptions AndParameter(string parameterName, object value) {
            if (parameterName == null) {
                throw new ArgumentNullException(nameof(parameterName));
            }

            return new WithParameterOptions(parameterName, value, ComponentRegistration);
        }
    }
}