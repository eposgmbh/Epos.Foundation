using System;

namespace Epos.CommandLine.Application;

/// <summary> Marks a parameter property on a subcommand class,
/// see <see cref="ISubcommand"/> interface.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ParameterAttribute : Attribute
{
    /// <summary> Initializes an instance of the <see cref="ParameterAttribute"/>
    /// class associating the marked property with the specified parameter name. </summary>
    /// <param name="name">Parameter name</param>
    /// <param name="description">Parameter description</param>
    public ParameterAttribute(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name)) {
            throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
        }
        if (string.IsNullOrWhiteSpace(description)) {
            throw new ArgumentException($"'{nameof(description)}' cannot be null or whitespace.", nameof(description));
        }

        Name = name;
        Description = description;
    }

    /// <summary> Gets the parameter name. </summary>
    public string Name { get; }

    /// <summary> Gets the description. </summary>
    public string Description { get; }

    /// <summary> Gets or sets the default value </summary>
    public object? DefaultValue { get; set; }
}
