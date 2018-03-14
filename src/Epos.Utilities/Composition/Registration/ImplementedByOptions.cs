using System;

namespace Epos.Utilities.Composition
{
    /// <summary>Options following the <see cref="RegisterOptions{TAbstraction}.ImplementedBy{TImplementation}"/>
    /// fluent interface step.
    /// <list type="bullet">
    /// <item><description><see cref="WithParameter"/></description></item>
    /// </list>
    /// </summary>
    /// <typeparam name="TAbstraction">Abstraction</typeparam>
    /// <typeparam name="TImplementation">Implementation</typeparam>
    public sealed class ImplementedByOptions<TAbstraction, TImplementation> : LifetimeOptions where TImplementation : TAbstraction
    {
        internal ImplementedByOptions(ComponentRegistration componentRegistration) : base(componentRegistration) {
            ComponentRegistration.ConcreteType = typeof (TImplementation);
        }

        /// <summary>Specifies the first constructor parameter of the implementation.
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="value">Parameter value</param>
        /// <returns>Options following this fluent interface step</returns>
        public WithParameterOptions WithParameter(string parameterName, object value) {
            if (parameterName == null) {
                throw new ArgumentNullException(nameof(parameterName));
            }

            return new WithParameterOptions(parameterName, value, ComponentRegistration);
        }
    }
}
