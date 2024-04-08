using System;

namespace Epos.CommandLine;

/// <summary> Marks an option property on an option class
/// used in <see cref="CommandLineSubcommand{TOptions}"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class CommandLineOptionAttribute : Attribute
{
    /// <summary> Initializes an instance of the <see cref="CommandLineOptionAttribute"/>
    /// class associating the marked property with the specified option letter. </summary>
    /// <param name="letter">Option letter</param>
    public CommandLineOptionAttribute(char letter)
    {
        Letter = letter;
    }

    /// <summary> Gets the option letter. </summary>
    public char Letter { get; }
}
