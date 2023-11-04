using System;
using Epos.Utilities;

using static Epos.Utilities.Characters;

namespace Epos.CommandLine.Sample
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var theCommandLineDefinition = new CommandLineDefinition
            {
                Name = "sample", // <- if null, .exe-Filename is taken
                Subcommands = {
                    new CommandLineSubcommand<BuildOptions>("build", "Builds something.") {
                        Options = {
                            new CommandLineOption<int>('p', "Sets the project number.") { LongName = "project-number" },
                            new CommandLineOption<string>('m', "Sets the used memory.") {
                                LongName = "memory",
                                DefaultValue = "1 GB"
                            },
                            new CommandLineSwitch('d', "Disables the command."),
                            new CommandLineSwitch('z', "Zzzz...")
                        },
                        Parameters = {
                            new CommandLineParameter<string>("filename", "Sets the filename.")
                        },
                        CommandLineFunc = (options, definition) => {
                            // Do something for the build subcommand
                            // ...

                            Console.WriteLine("sample command line application" + Lf);
                            Console.WriteLine(options.Dump() + Lf);

                            return 0; // <- your error code or 0, if successful
                        }
                    },
                    new CommandLineSubcommand<TestOptions>("test", "Tests something.") {
                        Options = {
                            new CommandLineSwitch('h', "Shows help for this subcommand.") { LongName = "help" }
                        },
                        CommandLineFunc = (options, definition) => {
                            if (options.ShowHelp) {
                                definition.ShowHelp("test");
                            } else {
                                Console.WriteLine("sample command line application" + Lf);
                                Console.WriteLine("Subcommand test was invoked." + Lf);
                            }

                            return 0;
                        }
                    }
                }
            };

            return theCommandLineDefinition.Try(args);
        }
    }
}
