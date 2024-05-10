using System;

namespace Epos.CommandLine.Application;

/// <summary> Marks an option property on a subcommand class,
/// see <see cref="ISubcommand"/> interface.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class OptionAttribute : Attribute
{
    /// <summary> Initializes an instance of the <see cref="OptionAttribute"/>
    /// class associating the marked property with the specified option letter. </summary>
    /// <param name="letter">Option letter</param>
    /// <param name="description">Option description</param>
    public OptionAttribute(char letter, string description) {
        if (string.IsNullOrWhiteSpace(description)) {
            throw new ArgumentException($"'{nameof(description)}' cannot be null or whitespace.", nameof(description));
        }

        Letter = letter;
        Description = description;
    }

    /// <summary> Gets the option letter. </summary>
    public char Letter { get; }

    /// <summary> Gets or sets the long name without the double dash ("example" for "--example").
    /// </summary>
    public string? LongName { get; set; }

    /// <summary> Gets the option description. </summary>
    public string Description { get; }

    /// <summary> Gets or sets the default value </summary>
    public object? DefaultValue { get; set; }

    /// <summary> Gets or sets the exclusion groups to which this option belongs. </summary>
    public string[]? ExclusionGroups { get; set; }
}
