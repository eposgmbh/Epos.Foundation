using System.Collections.Generic;
using System.Linq;

namespace Epos.CommandLine;

internal sealed class CommandLineTokenizer
{
    private readonly CommandLineDefinition myDefinition;
    private readonly CommandLineUsageWriter myUsageWriter;

    public CommandLineTokenizer(CommandLineDefinition definition, CommandLineUsageWriter usageWriter) {
        myDefinition = definition;
        myUsageWriter = usageWriter;
    }

    public List<CommandLineToken> Tokenize(string[] args, ref CommandLineSubcommand? subcommand) {
        var theResult = new List<CommandLineToken>();
        var theExclusionGroupCounts = new Dictionary<string, List<CommandLineOption>>();

        int theParameterIndex = 0;
        for (int theIndex = 0; theIndex < args.Length; theIndex++) {
            string theArg = args[theIndex];
            if (theIndex == 0 && myDefinition.HasDifferentiatedSubcommands) {
                // Muss Subcommand sein
                subcommand = myDefinition.Subcommands.SingleOrDefault(sc => sc.Name == theArg);
                if (subcommand is not null) {
                    theResult.Add(new CommandLineToken(CommandLineTokenKind.Subcommand, theArg));
                    continue;
                } else {
                    if (theArg.StartsWith("-")) {
                        myUsageWriter.WriteAndExit("Subcommand is missing.");
                    } else {
                        myUsageWriter.WriteAndExit($"Unknown subcommand: {theArg}");
                    }
                }
            }

            if (theArg.StartsWith("-")) {
                // Option
                if (subcommand is null) {
                    myUsageWriter.WriteAndExit();
                } else {
                    bool isLongNameOption = theArg.StartsWith("--");

                    string theOptionTextWithoutDashes = isLongNameOption ? theArg.Substring(2) : theArg.Substring(1);

                    CommandLineOption? theOption =
                        isLongNameOption ?
                            subcommand.Options.SingleOrDefault(o => o.LongName == theOptionTextWithoutDashes) :
                            subcommand.Options.SingleOrDefault(o => o.Letter.ToString() == theOptionTextWithoutDashes);

                    if (theOption is not null) {
                        foreach (string theExclusionGroup in theOption.ExclusionGroups) {
                            if (theExclusionGroupCounts.TryGetValue(theExclusionGroup,
                                out List<CommandLineOption>? theOptions)) {
                                theOptions.Add(theOption);
                                theExclusionGroupCounts[theExclusionGroup] = theOptions;
                            } else {
                                theOptions = new List<CommandLineOption> { theOption };
                                theExclusionGroupCounts[theExclusionGroup] = theOptions;
                            }
                        }

                        object? theValue = true;
                        if (!theOption.IsSwitch) {
                            if (theIndex < args.Length - 1) {
                                theIndex++;
                                string theRawValue = args[theIndex];

                                theValue = ParseUtils.ParseOption(theOption, theRawValue, out string? theErrorMessage);

                                if (theErrorMessage is not null) {
                                    myUsageWriter.WriteAndExit(subcommand, theErrorMessage);
                                }
                            } else {
                                // Value kommt nicht
                                myUsageWriter.WriteAndExit(
                                    subcommand,
                                    $"Missing value for option {theOption.ToLongCommandLineString()}."
                                );
                            }
                        }

                        theResult.Add(
                            new CommandLineToken(CommandLineTokenKind.Option, theOption.Letter.ToString()) {
                                Value = theValue!
                            }
                        );
                    } else {
                        // Sind es vielleicht mehrere verkettete Switches (-abc)?
                        if (!isLongNameOption && theOptionTextWithoutDashes.Length > 1) {
                            foreach (char theOptionLetter in theOptionTextWithoutDashes) {
                                theOption = subcommand.Options.SingleOrDefault(o => o.Letter == theOptionLetter);

                                if (theOption is not null && theOption.IsSwitch) {
                                    foreach (string theExclusionGroup in theOption.ExclusionGroups) {
                                        if (theExclusionGroupCounts.TryGetValue(theExclusionGroup,
                                            out List<CommandLineOption>? theOptions)) {
                                            theOptions.Add(theOption);
                                            theExclusionGroupCounts[theExclusionGroup] = theOptions;
                                        } else {
                                            theOptions = new List<CommandLineOption> { theOption };
                                            theExclusionGroupCounts[theExclusionGroup] = theOptions;
                                        }
                                    }

                                    theResult.Add(
                                        new CommandLineToken(CommandLineTokenKind.Option, theOptionLetter.ToString()) {
                                            Value = true
                                        }
                                    );
                                } else {
                                    myUsageWriter.WriteAndExit(subcommand, $"Unknown switch: -{theOptionLetter}");
                                }
                            }
                        } else {
                            // Unbekannte Option
                            myUsageWriter.WriteAndExit(subcommand, $"Unknown option: {theArg}");
                        }
                    }
                }
            } else {
                // Parameter
                if (subcommand is null) {
                    myUsageWriter.WriteAndExit();
                } else {
                    if (theParameterIndex >= subcommand.Parameters.Count) {
                        myUsageWriter.WriteAndExit(
                            subcommand, "Too many parameters: " + string.Join(" ", args.Skip(theIndex))
                        );
                    }

                    CommandLineParameter theParameter = subcommand.Parameters[theParameterIndex++];

                    string theRawValue = args[theIndex];

                    object? theValue = ParseUtils.ParseParameter(
                        theParameter, theRawValue, out string? theErrorMessage
                    );

                    if (theErrorMessage is not null) {
                        myUsageWriter.WriteAndExit(subcommand, theErrorMessage);
                    }

                    theResult.Add(
                        new CommandLineToken(CommandLineTokenKind.Parameter, theParameter.Name) {
                            Value = theValue!
                        }
                    );
                }
            }
        }

        foreach (List<CommandLineOption> theOptions in theExclusionGroupCounts.Values) {
            if (theOptions.Count > 1) {
                string theOptionStrings =
                    theOptions.Select(o => o.ToLongCommandLineString()).Aggregate((s1, s2) => s1 + ", " + s2);

                myUsageWriter.WriteAndExit(
                    subcommand!,
                    $"Only one of the following options may be set: {theOptionStrings}"
                );
            }
        }

        // Schauen, ob zuerst der SubcommandName, dann Options, dann Parameter gesetzt sind
        ValidateCommandLineTokens(theResult, subcommand!);

        // Option Defaults setzen
        FillOptionDefaults(theResult, subcommand!, theExclusionGroupCounts.Keys);

        // Schauen, ob alle benötigten Parameter gesetzt sind
        FillOptionalParameterDefaults(theResult, subcommand!);

        return theResult;
    }

    private void ValidateCommandLineTokens(
        IEnumerable<CommandLineToken> commandLineTokens, CommandLineSubcommand subcommand
    ) {
        bool isOption = false, isParameter = false;
        foreach (CommandLineToken theArgToken in commandLineTokens) {
            if (theArgToken.Kind == CommandLineTokenKind.Option) {
                isOption = true;

                if (isParameter) {
                    // Parameter vor Option
                    myUsageWriter.WriteAndExit(subcommand, "Order of options and parameters is wrong.");
                }
            } else if (theArgToken.Kind == CommandLineTokenKind.Parameter) {
                isParameter = true;
            } else {
                // SubcommandName
                if (isOption || isParameter) {
                    // Parameter oder Option vor SubcommandName
                    myUsageWriter.WriteAndExit(subcommand, "Order of options and parameters is wrong.");
                }
            }
        }
    }

    private void FillOptionDefaults(
        ICollection<CommandLineToken> commandLineTokens, CommandLineSubcommand subcommand,
        IEnumerable<string> exclusionGroups
    ) {
        foreach (CommandLineOption theOption in subcommand.Options) {
            object? theDefaultValue = theOption.GetDefaultValue();

            if (theDefaultValue is not null) {
                if (!commandLineTokens.Any(t => t.Kind == CommandLineTokenKind.Option &&
                    t.Name == theOption.Letter.ToString())) {
                    commandLineTokens.Add(
                        new CommandLineToken(CommandLineTokenKind.Option, theOption.Letter.ToString()) {
                            Value = theDefaultValue
                        }
                    );
                }
            } else {
                // Kein Default value => Option muss gesetzt sein (außer bei Switch, und falls exclusion group
                // bereits erfuellt)
                if (!theOption.IsSwitch &&
                    !commandLineTokens.Any(t => t.Kind == CommandLineTokenKind.Option &&
                    t.Name == theOption.Letter.ToString()) &&
                   (!theOption.ExclusionGroups.Any() || theOption.ExclusionGroups.Except(exclusionGroups).Any())) {
                    myUsageWriter.WriteAndExit(subcommand, $"Missing option: {theOption.ToShortCommandLineString()}");
                }
            }
        }
    }

    private void FillOptionalParameterDefaults(
        ICollection<CommandLineToken> commandLineTokens, CommandLineSubcommand subcommand
    ) {
        foreach (CommandLineParameter theParameter in subcommand.Parameters) {
            if (!commandLineTokens.Any(t => t.Kind == CommandLineTokenKind.Parameter && t.Name == theParameter.Name)) {
                if (!theParameter.IsOptional) {
                    myUsageWriter.WriteAndExit(subcommand, $"Missing parameter: {theParameter.Name}");
                } else {
                    // Optionaler Parameter
                    commandLineTokens.Add(
                        new CommandLineToken(CommandLineTokenKind.Parameter, theParameter.Name) {
                            Value = theParameter.GetDefaultValue()!
                        }
                    );
                }
            }
        }
    }
}
