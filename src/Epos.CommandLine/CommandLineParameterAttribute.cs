using System;

namespace Epos.CommandLine;

/// <summary> Marks a parameter property on an option class
/// used in <see cref="CommandLineSubcommand{TOptions}"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class CommandLineParameterAttribute : Attribute
{
    /// <summary> Initializes an instance of the <see cref="CommandLineParameterAttribute"/>
    /// class associating the marked property with the specified parameter name. </summary>
    /// <param name="name">Parameter name</param>
    public CommandLineParameterAttribute(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    /// <summary> Gets the parameter name. </summary>
    public string Name { get; }
}
