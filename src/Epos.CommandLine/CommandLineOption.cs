using System;
using System.Collections.Generic;
using System.Text;

using Epos.Utilities;

namespace Epos.CommandLine;

/// <summary> Command line option, see <a href="/getting-started.html">Getting started</a>
/// for an example.</summary>
/// <typeparam name="T">Option data type</typeparam>
public class CommandLineOption<T> : CommandLineOption
{
    private T myDefaultValue = default!;
    private bool myIsDefaultValueSet;

    static CommandLineOption() {
        typeof(T).TestAvailable();
    }

    /// <summary> Initializes an instance of the <see cref="CommandLineOption{T}"/> class.
    /// </summary>
    /// <param name="letter">Option letter</param>
    /// <param name="description">Description</param>
    public CommandLineOption(char letter, string description) : base(typeof(T), letter, description) { }

    /// <summary> Gets or sets the default value that is used, if the option is not
    /// specified on the command line.</summary>
    public T DefaultValue {
        get => myDefaultValue;
        set {
            myDefaultValue = value;
            myIsDefaultValueSet = true;
        }
    }

    internal override object? GetDefaultValue() => myIsDefaultValueSet ? (object?) DefaultValue : null;
}

/// <summary> Command line option base class.</summary>
public abstract class CommandLineOption
{
    /// <summary> Initializes an instance of the <see cref="CommandLineOption"/> class.
    /// </summary>
    /// <param name="dataType">Option data type</param>
    /// <param name="letter">Option letter</param>
    /// <param name="description">Description</param>
    protected CommandLineOption(Type dataType, char letter, string description) {
        DataType = dataType ?? throw new ArgumentNullException(nameof(dataType));
        Letter = letter;
        Description = description ?? throw new ArgumentNullException(nameof(description));
    }

    /// <summary> Gets the option letter.</summary>
    public char Letter { get; }

    /// <summary> Gets the description. </summary>
    public string Description { get; }

    /// <summary> Gets the data type. </summary>
    public Type DataType { get; }

    /// <summary> Gets or sets the long name without the double dash ("example" for "--example").
    /// </summary>
    public string? LongName { get; set; }

    /// <summary> Exclusion groups to which this option belongs. </summary>
    /// <remarks> Two options with the same exclusion group cannot be used together. </remarks>
    public IList<string> ExclusionGroups { get; } = [];

    internal bool IsSwitch => DataType == typeof(bool);

    internal string ToShortCommandLineString() {
        string theOptionName = $"-{Letter}";

        if (LongName is not null) {
            theOptionName += $", --{LongName}";
        }

        return theOptionName;
    }

    internal string ToLongCommandLineString() {
        StringBuilder theResult = new StringBuilder()
            .Append('[')
            .Append(ToShortCommandLineString());

        if (!IsSwitch) {
            theResult
                .Append(" <")
                .Append(DataType.Dump());

            object? theDefaultValue = GetDefaultValue();
            if (theDefaultValue is not null) {
                theResult
                    .Append("=");

                if (theDefaultValue is string) {
                    theResult.Append('"');
                }

                theResult.Append(theDefaultValue.Dump());

                if (theDefaultValue is string) {
                    theResult.Append('"');
                }
            }

            theResult.Append('>');
        }

        theResult.Append(']');

        return theResult.ToString();
    }

    internal abstract object? GetDefaultValue();
}
