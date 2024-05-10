using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Epos.CommandLine.Application;
using Epos.Utilities;

namespace Epos.CommandLine;


/// <summary> Command line subcommand, see <a href="/getting-started.html">Getting started</a>
/// for an example.</summary>
/// <typeparam name="TOptions">Options data type</typeparam>
public sealed class CommandLineSubcommand<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties |
     DynamicallyAccessedMemberTypes.NonPublicProperties)] TOptions
> : CommandLineSubcommand where TOptions : class, new()
{
    /// <summary> Initializes an instance of the <see cref="CommandLineSubcommand{TOptions}"/> class.
    /// </summary>
    /// <param name="name">Subcommand name</param>
    /// <param name="description">Description</param>
    public CommandLineSubcommand(string name, string description) : base(name, description) { }

    /// <summary> Gets or sets the command line function. </summary>
    /// <remarks> The command line function is called with a <typeparamref name="TOptions"/>
    /// instance that is initialized with the command line arguments. The function is
    /// only called, if the command line can be parsed successfully. The function should
    /// return an error code (0 if successful). </remarks>
    public Func<TOptions, CommandLineDefinition, int> CommandLineFunc { get; set; } = (o, cd) => 0;

    /// <summary> Gets or sets the async command line function. </summary>
    /// <remarks> The command line function is called with a <typeparamref name="TOptions"/>
    /// instance that is initialized with the command line arguments. The function is
    /// only called, if the command line can be parsed successfully. The function should
    /// return an error code (0 if successful). If the async command line function is not set, the sync
    /// version (<see cref="CommandLineFunc"/>) is called. </remarks>
    public Func<TOptions, CommandLineDefinition, Task<int>>? AsyncCommandLineFunc { get; set; }

    internal override async Task<int> ExecuteAsync(
        IEnumerable<CommandLineToken> argTokens, CommandLineDefinition definition, CancellationToken cancellationToken) {
        var theOptions = new TOptions();
        Type theOptionsType = typeof(TOptions);

        foreach (CommandLineToken theArgToken in argTokens) {
            if (theArgToken.Kind != CommandLineTokenKind.Subcommand) {
                // Option oder Parameter
                string thePropertyName = theArgToken.Name;

                // Spezifische (Attribute) PropertyInfo finden
                PropertyInfo? thePropertyInfo;

                if (theArgToken.Kind == CommandLineTokenKind.Option) {
                    thePropertyName = thePropertyName.Replace("-", ""); // de-kebap-style

                    thePropertyInfo =
                        theOptionsType
                            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .SingleOrDefault(
                                pi => pi.GetCustomAttribute<CommandLineOptionAttribute>() is CommandLineOptionAttribute theAttribute &&
                                      theAttribute.Letter.ToString() == thePropertyName
                            );
                } else {
                    // Parameter
                    thePropertyInfo =
                        theOptionsType
                            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .SingleOrDefault(
                                pi => pi.GetCustomAttribute<CommandLineParameterAttribute>() is CommandLineParameterAttribute theAttribute &&
                                      theAttribute.Name.ToLower() == thePropertyName
                            );
                }

                // Falls nicht gefunden: Property mit genau dem passenden Namen (case-insensitive) finden
                thePropertyInfo ??= theOptionsType.GetProperty(
                    thePropertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase
                );

                if (thePropertyInfo is not null) {
                    // Property gefunden, Wert setzen
                    thePropertyInfo.SetMethod?.Invoke(theOptions, [theArgToken.Value]);
                } else {
                    throw new InvalidOperationException(
                        $"No property for \"{thePropertyName}\" found on options type {theOptionsType.FullName}."
                    );
                }
            }
        }

        if (AsyncCommandLineFunc is not null) {
            return await AsyncCommandLineFunc(theOptions, definition);
        } else {
            return CommandLineFunc.Invoke(theOptions, definition);
        }
    }
}

/// <summary> Command line subcommand base class.</summary>
public abstract class CommandLineSubcommand
{
    /// <summary> Default name for the single subcommand, if no
    /// differentiated subcommands are to be used. </summary>
    public const string DefaultName = "default";

    /// <summary> Initializes an instance of the <see cref="CommandLineSubcommand"/> class.
    /// </summary>
    /// <param name="name">Subcommand name</param>
    /// <param name="description">Description</param>
    protected CommandLineSubcommand(string name, string description) {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));

        Options = new List<CommandLineOption>();
        Parameters = new List<CommandLineParameter>();
    }

    /// <summary> Gets the subcommand name.</summary>
    public string Name { get; }

    /// <summary> Gets the subcommand description.</summary>
    public string Description { get; }

    /// <summary> Gets the subcommand options. </summary>
    /// <remarks> Here you can register your subcommand options. </remarks>
    public IList<CommandLineOption> Options { get; }

    /// <summary> Gets the subcommand parameters. </summary>
    /// <remarks> Here you can register your subcommand parameters. </remarks>
    public IList<CommandLineParameter> Parameters { get; }

    internal abstract Task<int> ExecuteAsync(
        IEnumerable<CommandLineToken> argTokens, CommandLineDefinition definition, CancellationToken cancellationToken);
}

internal sealed class CommandLineSubcommandWrapper : CommandLineSubcommand
{
    private readonly ISubcommand mySubcommand;

    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Types are available at runtime.")]
    public CommandLineSubcommandWrapper(ISubcommand subcommand) : base(subcommand.Name, subcommand.Description) {
        foreach (PropertyInfo thePropertyInfo in subcommand.GetType().GetProperties()) {
            OptionAttribute? theOptionAttribute = thePropertyInfo.GetAttribute<OptionAttribute>();
            if (theOptionAttribute is not null) {
                Type theCommandLineOptionType =
                    typeof(CommandLineOption<>).MakeGenericType(thePropertyInfo.PropertyType);
                ConstructorInfo theConstructor =
                    theCommandLineOptionType.GetConstructor([typeof(char), typeof(string)])!;

                var theOption =
                    (CommandLineOption) theConstructor.Invoke([theOptionAttribute.Letter, theOptionAttribute.Description]);
                theOption.LongName = theOptionAttribute.LongName;

                if (theOptionAttribute.DefaultValue is not null) {
                    theOption
                        .GetType()
                        .GetProperty("DefaultValue")!
                        .GetSetMethod()!
                        .Invoke(theOption, [theOptionAttribute.DefaultValue]);
                }

                if (theOptionAttribute.ExclusionGroups is not null) {
                    foreach (string theExclusionGroup in theOptionAttribute.ExclusionGroups) {
                        theOption.ExclusionGroups.Add(theExclusionGroup);
                    }
                }

                Options.Add(theOption);
            }

            ParameterAttribute? theParameterAttribute = thePropertyInfo.GetAttribute<ParameterAttribute>();
            if (theParameterAttribute is not null) {
                Type theCommandLineParameterType =
                    typeof(CommandLineParameter<>).MakeGenericType(thePropertyInfo.PropertyType);
                ConstructorInfo theConstructor =
                    theCommandLineParameterType.GetConstructor([typeof(string), typeof(string)])!;

                var theParameter =
                    (CommandLineParameter) theConstructor.Invoke([theParameterAttribute.Name, theParameterAttribute.Description]);

                if (theParameterAttribute.DefaultValue is not null) {
                    theParameter
                        .GetType()
                        .GetProperty("DefaultValue")!
                        .GetSetMethod()!
                        .Invoke(theParameter, [theParameterAttribute.DefaultValue]);
                }

                Parameters.Add(theParameter);
            }
        }

        mySubcommand = subcommand;
    }

    internal override async Task<int> ExecuteAsync(
        IEnumerable<CommandLineToken> argTokens, CommandLineDefinition definition, CancellationToken cancellationToken) {
        ISubcommand theSubcommand = mySubcommand;
        Type theSubcommandType = theSubcommand.GetType();

        foreach (CommandLineToken theArgToken in argTokens) {
            if (theArgToken.Kind != CommandLineTokenKind.Subcommand) {
                // Option oder Parameter
                string thePropertyName = theArgToken.Name;

                // Spezifische (Attribute) PropertyInfo finden
                PropertyInfo? thePropertyInfo;

                if (theArgToken.Kind == CommandLineTokenKind.Option) {
                    thePropertyName = thePropertyName.Replace("-", ""); // de-kebap-style

                    thePropertyInfo =
                        theSubcommandType
                            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .SingleOrDefault(
                                pi => pi.GetCustomAttribute<OptionAttribute>() is OptionAttribute theAttribute &&
                                      theAttribute.Letter.ToString() == thePropertyName);
                } else {
                    // Parameter
                    thePropertyInfo =
                        theSubcommandType
                            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .SingleOrDefault(
                                pi => pi.GetCustomAttribute<ParameterAttribute>() is ParameterAttribute theAttribute &&
                                      theAttribute.Name.ToLower() == thePropertyName);
                }

                // Falls nicht gefunden: Property mit genau dem passenden Namen (case-insensitive) finden
                thePropertyInfo ??= theSubcommandType.GetProperty(
                    thePropertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase
                );

                if (thePropertyInfo is not null) {
                    // Property gefunden, Wert setzen
                    thePropertyInfo.SetMethod?.Invoke(theSubcommand, [theArgToken.Value]);
                } else {
                    throw new InvalidOperationException(
                        $"No property for \"{thePropertyName}\" found on options type {theSubcommandType.FullName}."
                    );
                }
            }
        }

        return await mySubcommand.ExecuteAsync(cancellationToken);
    }
}
