using System;

namespace Epos.Utilities.Composition;

/// <summary>Options following the <see cref="ImplementedByOptions{TAbstract,TConcrete}.WithParameter"/>
/// fluent interface step.
/// <list type="bullet">
/// <item><description><see cref="AndParameter"/></description></item>
/// </list>
/// </summary>
public sealed class WithParameterOptions : LifetimeOptions
{
    internal WithParameterOptions(string parameterName, object value, ComponentRegistration componentRegistration)
        : base(componentRegistration) {
        ComponentRegistration.Parameters[parameterName] = value;
    }

    /// <summary>Specifies another constructor parameter of the implementation.
    /// </summary>
    /// <param name="parameterName">Parameter name</param>
    /// <param name="value">Parameter value</param>
    /// <returns>Options following this fluent interface step</returns>
    public WithParameterOptions AndParameter(string parameterName, object value) {
        if (parameterName is null) {
            throw new ArgumentNullException(nameof(parameterName));
        }

        return new WithParameterOptions(parameterName, value, ComponentRegistration);
    }
}
