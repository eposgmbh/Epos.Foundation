using System.Collections.Generic;
using System.Linq;

namespace Epos.CmdLine
{
    internal sealed class CmdLineTokenizer
    {
        private readonly CmdLineDefinition myDefinition;
        private readonly CmdLineUsageWriter myUsageWriter;

        public CmdLineTokenizer(CmdLineDefinition definition, CmdLineUsageWriter usageWriter) {
            myDefinition = definition;
            myUsageWriter = usageWriter;
        }

        public List<CmdLineToken> Tokenize(string[] args, ref CmdLineSubcommand subcommand) {
            var theResult = new List<CmdLineToken>();
            var theExclusionGroupCounts = new Dictionary<string, List<CmdLineOption>>();

            int theParameterIndex = 0;
            for (int theIndex = 0; theIndex < args.Length; theIndex++) {
                string theArg = args[theIndex];
                if (theIndex == 0 && myDefinition.HasDifferentiatedSubcommands) {
                    // Muss Subcommand sein
                    subcommand = myDefinition.Subcommands.SingleOrDefault(sc => sc.Name == theArg);
                    if (subcommand != null) {
                        theResult.Add(new CmdLineToken(CmdLineTokenKind.Subcommand, theArg));
                        continue;
                    }
                    else {
                        if (theArg.StartsWith("-")) {
                            myUsageWriter.WriteAndExit("Subcommand is missing.");
                        } else {
                            myUsageWriter.WriteAndExit($"Unknown subcommand: {theArg}");
                        }
                    }
                }

                if (theArg.StartsWith("-")) {
                    // Option
                    if (subcommand == null) {
                        myUsageWriter.WriteAndExit();
                    } else {
                        bool isLongNameOption = theArg.StartsWith("--");

                        string theOptionTextWithoutDashes = isLongNameOption ? theArg.Substring(2) : theArg.Substring(1);

                        CmdLineOption theOption =
                            isLongNameOption ?
                                subcommand.Options.SingleOrDefault(o => o.LongName == theOptionTextWithoutDashes) :
                                subcommand.Options.SingleOrDefault(o => o.Letter.ToString() == theOptionTextWithoutDashes);

                        if (theOption != null) {
                            foreach (string theExclusionGroup in theOption.ExclusionGroups) {
                                if (theExclusionGroupCounts.TryGetValue(theExclusionGroup,
                                    out List<CmdLineOption> theOptions)) {
                                    theOptions.Add(theOption);
                                    theExclusionGroupCounts[theExclusionGroup] = theOptions;
                                } else {
                                    theOptions = new List<CmdLineOption> { theOption };
                                    theExclusionGroupCounts[theExclusionGroup] = theOptions;
                                }
                            }

                            object theValue = true;
                            if (!theOption.IsSwitch) {
                                if (theIndex < args.Length - 1) {
                                    theIndex++;
                                    string theRawValue = args[theIndex];

                                    theValue = ParseUtils.ParseOption(theOption, theRawValue, out string theErrorMessage);

                                    if (theErrorMessage != null) {
                                        myUsageWriter.WriteAndExit(subcommand, theErrorMessage);
                                    }
                                } else {
                                    // Value kommt nicht
                                    myUsageWriter.WriteAndExit(
                                        subcommand,
                                        $"Missing value for option {theOption.ToLongCmdLineString()}."
                                    );
                                }
                            }

                            theResult.Add(
                                new CmdLineToken(CmdLineTokenKind.Option, theOption.Letter.ToString()) {
                                    Value = theValue
                                }
                            );
                        } else {
                            // Sind es vielleicht mehrere verkettete Switches (-abc)?
                            if (!isLongNameOption && theOptionTextWithoutDashes.Length > 1) {
                                foreach (char theOptionLetter in theOptionTextWithoutDashes) {
                                    theOption = subcommand.Options.SingleOrDefault(o => o.Letter == theOptionLetter);

                                    if (theOption != null && theOption.IsSwitch) {
                                        foreach (string theExclusionGroup in theOption.ExclusionGroups) {
                                            if (theExclusionGroupCounts.TryGetValue(theExclusionGroup,
                                                out List<CmdLineOption> theOptions)) {
                                                theOptions.Add(theOption);
                                                theExclusionGroupCounts[theExclusionGroup] = theOptions;
                                            } else {
                                                theOptions = new List<CmdLineOption> { theOption };
                                                theExclusionGroupCounts[theExclusionGroup] = theOptions;
                                            }
                                        }

                                        theResult.Add(
                                            new CmdLineToken(CmdLineTokenKind.Option, theOptionLetter.ToString()) {
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
                    if (subcommand == null) {
                        myUsageWriter.WriteAndExit();
                    } else {
                        if (theParameterIndex >= subcommand.Parameters.Count) {
                            myUsageWriter.WriteAndExit(
                                subcommand, "Too many parameters: " + string.Join(" ", args.Skip(theIndex))
                            );
                        }

                        CmdLineParameter theParameter = subcommand.Parameters[theParameterIndex++];

                        string theRawValue = args[theIndex];

                        object theValue = ParseUtils.ParseParameter(
                            theParameter, theRawValue, out string theErrorMessage
                        );

                        if (theErrorMessage != null) {
                            myUsageWriter.WriteAndExit(subcommand, theErrorMessage);
                        }

                        theResult.Add(
                            new CmdLineToken(CmdLineTokenKind.Parameter, theParameter.Name) {
                                Value = theValue
                            }
                        );
                    }
                }
            }

            foreach (List<CmdLineOption> theOptions in theExclusionGroupCounts.Values) {
                if (theOptions.Count > 1) {
                    string theOptionStrings =
                        theOptions.Select(o => o.ToLongCmdLineString()).Aggregate((s1, s2) => s1 + ", " + s2);

                    myUsageWriter.WriteAndExit(
                        subcommand,
                        $"Only one of the following options may be set: {theOptionStrings}"
                    );
                }
            }

            // Schauen, ob zuerst der SubcommandName, dann Options, dann Parameter gesetzt sind
            ValidateCmdLineTokens(theResult, subcommand);

            // Option Defaults setzen
            FillOptionDefaults(theResult, subcommand);

            // Schauen, ob alle benötigten Parameter gesetzt sind
            FillOptionalParameterDefaults(theResult, subcommand);

            return theResult;
        }

        private void ValidateCmdLineTokens(IEnumerable<CmdLineToken> cmdLineTokens, CmdLineSubcommand subcommand) {
            bool isOption = false, isParameter = false;
            foreach (CmdLineToken theArgToken in cmdLineTokens) {
                if (theArgToken.Kind == CmdLineTokenKind.Option) {
                    isOption = true;

                    if (isParameter) {
                        // Parameter vor Option
                        myUsageWriter.WriteAndExit(subcommand, "Order of options and parameters is wrong.");
                    }
                } else if (theArgToken.Kind == CmdLineTokenKind.Parameter) {
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

        private void FillOptionDefaults(ICollection<CmdLineToken> cmdLineTokens, CmdLineSubcommand subcommand) {
            foreach (CmdLineOption theOption in subcommand.Options) {
                object theDefaultValue = theOption.GetDefaultValue();

                if (theDefaultValue != null) {
                    if (!cmdLineTokens.Any(t => t.Kind == CmdLineTokenKind.Option && t.Name == theOption.Letter.ToString())) {
                        cmdLineTokens.Add(
                            new CmdLineToken(CmdLineTokenKind.Option, theOption.Letter.ToString()) {
                                Value = theDefaultValue
                            }
                        );
                    }
                } else {
                    // Kein Default value => Option muss gesetzt sein (außer bei Switch)
                    if (!theOption.IsSwitch &&
                        !cmdLineTokens.Any(t => t.Kind == CmdLineTokenKind.Option &&
                        t.Name == theOption.Letter.ToString())) {
                        myUsageWriter.WriteAndExit(subcommand, $"Missing option: {theOption.ToShortCmdLineString()}");
                    }
                }
            }
        }

        private void FillOptionalParameterDefaults(ICollection<CmdLineToken> cmdLineTokens, CmdLineSubcommand subcommand) {
            foreach (CmdLineParameter theParameter in subcommand.Parameters) {
                if (!cmdLineTokens.Any(t => t.Kind == CmdLineTokenKind.Parameter && t.Name == theParameter.Name)) {
                    if (!theParameter.IsOptional) {
                        myUsageWriter.WriteAndExit(subcommand, $"Missing parameter: {theParameter.Name}");
                    } else {
                        // Optionaler Parameter
                        cmdLineTokens.Add(
                            new CmdLineToken(CmdLineTokenKind.Parameter, theParameter.Name) {
                                Value = theParameter.GetDefaultValue()
                            }
                        );
                    }
                }
            }
        }
    }
}
