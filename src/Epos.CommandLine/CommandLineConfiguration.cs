using System;
using System.IO;

namespace Epos.CommandLine;

/// <summary>Configuration class for the
/// <see cref="CommandLineDefinition"/>.</summary>
public sealed class CommandLineConfiguration
{
    /// <summary> Gets or sets the name of the command line tool. If not specified
    /// (<b>null</b>) the lower-case filename (without extension) is used.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>Specifies the <see cref="System.IO.TextWriter"/> that is used
    /// to write usage help.</summary>
    /// <remarks>Defaults to <see cref="System.Console.Out"/>.</remarks>
    public TextWriter UsageTextWriter { get; set; } = Console.Out;

    /// <summary>Specifies the action that is taken when the command line
    /// cannot be parsed successfully.</summary>
    /// <remarks>Defaults to:
    /// <code>Environment.Exit(-1);</code>
    /// </remarks>
    public Action ErrorAction { get; set; } = () => { Environment.Exit(-1); };
}
