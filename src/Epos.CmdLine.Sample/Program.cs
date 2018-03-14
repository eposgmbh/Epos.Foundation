using System;
using Epos.Utilities;

using static Epos.Utilities.Characters;

namespace Epos.CmdLine.Sample
{
    public static class Program
    {
        public static int Main(string[] args) {
            var theCmdLineDefinition = new CmdLineDefinition
            {
                Name = "sample", // <- if null, .exe-Filename is taken
                Subcommands = {
                    new CmdLineSubcommand<BuildOptions>("build", "Builds something.") {
                        Options = {
                            new CmdLineOption<int>('p', "Sets the project number.") { LongName = "project-number" },
                            new CmdLineOption<string>('m', "Sets the used memory.") {
                                LongName = "memory",
                                DefaultValue = "1 GB"
                            },
                            new CmdLineSwitch('d', "Disables the command."),
                            new CmdLineSwitch('z', "Zzzz...")
                        },
                        Parameters = {
                            new CmdLineParameter<string>("filename", "Sets the filename.")
                        },
                        CmdLineFunc = (options, definition) => {
                            // Do something for the build subcommand
                            // ...

                            Console.WriteLine("sample command line application" + Lf);
                            Console.WriteLine(options.Dump() + Lf);

                            return 0; // <- your error code or 0, if successful
                        }
                    },
                    new CmdLineSubcommand<TestOptions>("test", "Tests something.") {
                        Options = {
                            new CmdLineSwitch('h', "Shows help for this subcommand.") { LongName = "help" }
                        },
                        CmdLineFunc = (options, definition) => {
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

            return theCmdLineDefinition.Try(args);
        }
    }
}
