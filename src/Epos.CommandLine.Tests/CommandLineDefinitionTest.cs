using System;
using System.IO;
using NUnit.Framework;

using static Epos.Utilities.Characters;

namespace Epos.CommandLine
{
    [TestFixture]
    public class CommandLineDefinitionTest
    {
        [Test]
        public void BasicsWithoutSubcommand()
        {
            var theConsoleOutput = new StringWriter();

            var theCommandLineDefinition = new CommandLineDefinition
            {
                HasDifferentiatedSubcommands = false,
                Configuration = new CommandLineConfiguration
                {
                    UsageTextWriter = theConsoleOutput,
                    ErrorAction = () => { }
                }
            };

            Assert.Throws<ArgumentNullException>(() => theCommandLineDefinition.Try(null!));

            Assert.Throws<ArgumentNullException>(() => theCommandLineDefinition.Try([null!]));

            Assert.Throws<InvalidOperationException>(() => theCommandLineDefinition.Try(["non-existing-subcommand"]));

            bool isRun = false;
            theCommandLineDefinition.Subcommands.Add(
                new CommandLineSubcommand<object>(CommandLineSubcommand.DefaultName, "Default command")
                {
                    CommandLineFunc = (o, _) => { isRun = true; return 0; }
                }
            );

            theCommandLineDefinition.Try([]);

            Assert.That(theConsoleOutput.ToString(), Is.Empty);
            Assert.That(isRun, Is.True);
        }

        [Test]
        public void BasicsWithDifferentiatedCommands()
        {
            var theConsoleOutput = new StringWriter();

            var theCommandLineDefinition = new CommandLineDefinition
            {
                Name = "sample",
                Configuration = new CommandLineConfiguration
                {
                    UsageTextWriter = theConsoleOutput,
                    ErrorAction = () => { }
                }
            };

            Assert.That(theCommandLineDefinition.Name, Is.EqualTo("sample"));

            Assert.Throws<InvalidOperationException>(() => theCommandLineDefinition.Try([]));

            theCommandLineDefinition.Subcommands.Add(
                new CommandLineSubcommand<object>("build", "Builds something.")
            );
            theCommandLineDefinition.Subcommands.Add(
                new CommandLineSubcommand<object>("test", "Tests something.")
            );

            Assert.That(theCommandLineDefinition.Subcommands[0].Name, Is.EqualTo("build"));
            Assert.That(theCommandLineDefinition.Subcommands[1].Description, Is.EqualTo("Tests something."));
            Assert.That(theCommandLineDefinition.Subcommands[0].Options, Is.Empty);
            Assert.That(theCommandLineDefinition.Subcommands[1].Parameters, Is.Empty);

            theCommandLineDefinition.Try([]);

            Assert.That(
                theConsoleOutput.ToString(),
                Is.EqualTo(
                    "Usage: sample <build | test>" + DbLf +
                    "Subcommands" + Lf +
                    "  build   Builds something." + Lf +
                    "  test    Tests something." + DbLf
                )
            );
        }

        [Test]
        public void Parameters()
        {
            var theConsoleOutput = new StringWriter();

            // decimal als Parametertyp ist beispielsweise nicht erlaubt:

            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<TypeInitializationException>(() => new CommandLineParameter<decimal>(null!, null!));

            BuildOptions? theBuildOptions = null;

            var theCommandLineDefinition = new CommandLineDefinition
            {
                Name = "sample",
                Configuration = new CommandLineConfiguration
                {
                    UsageTextWriter = theConsoleOutput,
                    ErrorAction = () => throw new CommandLineError()
                },
                Subcommands = {
                    new CommandLineSubcommand<BuildOptions>("build", "Builds something.") {
                        Parameters = {
                            new CommandLineParameter<int>("project-number", "Sets the project number."),
                            new CommandLineParameter<string>("memory", "Sets the used memory.") {
                                DefaultValue = "1 GB"
                            }
                        },
                        CommandLineFunc = (o, _) => {
                            theBuildOptions = o;
                            return 0;
                        }
                    }
                }
            };

            Assert.Throws<CommandLineError>(() => theCommandLineDefinition.Try(["test"]));

            Assert.That(
                theConsoleOutput.ToString(),
                Is.EqualTo(
                    "Usage: sample <build>" + DbLf +
                    "Error: Unknown subcommand: test" + DbLf +
                    "Subcommands" + Lf +
                    "  build   Builds something." + DbLf
                )
            );

            theConsoleOutput.GetStringBuilder().Clear();

            Assert.Throws<CommandLineError>(() => theCommandLineDefinition.Try(["build"]));

            Assert.That(
                theConsoleOutput.ToString(),
                Is.EqualTo(
                    "Usage: sample build <project-number:int> [<memory:string>=\"1 GB\"]" + DbLf +
                    "Error: Missing parameter: project-number" + DbLf +
                    "Parameters" + Lf +
                    "  project-number   Sets the project number." + Lf +
                    "  memory           Sets the used memory. >>> defaults to \"1 GB\"" + DbLf
                )
            );

            theConsoleOutput.GetStringBuilder().Clear();

            Assert.Throws<CommandLineError>(() => theCommandLineDefinition.Try(["build", "no-number"]));

            Assert.That(
                theConsoleOutput.ToString(),
                Is.EqualTo(
                    "Usage: sample build <project-number:int> [<memory:string>=\"1 GB\"]" + DbLf +
                    "Error: Value \"no-number\" for parameter <project-number:int> is invalid." + DbLf +
                    "Parameters" + Lf +
                    "  project-number   Sets the project number." + Lf +
                    "  memory           Sets the used memory. >>> defaults to \"1 GB\"" + DbLf
                )
            );

            theConsoleOutput.GetStringBuilder().Clear();

            theCommandLineDefinition.Try(["build", "33"]);
            Assert.That(theBuildOptions!.ProjectNumber, Is.EqualTo(33));
            Assert.That(theBuildOptions.Memory, Is.EqualTo("1 GB"));
            Assert.That(theBuildOptions.DummyParameter, Is.Null);
            Assert.That(theConsoleOutput.ToString(), Is.Empty);

            theCommandLineDefinition.Try(["build", "66", "2 GB"]);
            Assert.That(theBuildOptions.ProjectNumber, Is.EqualTo(66));
            Assert.That(theBuildOptions.Memory, Is.EqualTo("2 GB"));
            Assert.That(theBuildOptions.DummyParameter, Is.Null);
            Assert.That(theConsoleOutput.ToString(), Is.Empty);

            Assert.Throws<CommandLineError>(() => theCommandLineDefinition.Try(["build", "33", "2 GB", "Too much!"]));

            Assert.That(
                theConsoleOutput.ToString(),
                Is.EqualTo(
                    "Usage: sample build <project-number:int> [<memory:string>=\"1 GB\"]" + DbLf +
                    "Error: Too many parameters: Too much!" + DbLf +
                    "Parameters" + Lf +
                    "  project-number   Sets the project number." + Lf +
                    "  memory           Sets the used memory. >>> defaults to \"1 GB\"" + DbLf
                )
            );
        }

        [Test]
        public void ExclusionGroups()
        {
            var theConsoleOutput = new StringWriter();

            TestOptions? theTestOptions = null;

            var theCommandLineDefinition = new CommandLineDefinition
            {
                Name = "sample",
                Configuration = new CommandLineConfiguration
                {
                    UsageTextWriter = theConsoleOutput,
                    ErrorAction = () => throw new CommandLineError()
                },
                Subcommands = {
                    new CommandLineSubcommand<TestOptions>("test", "Tests something.") {
                        Options = {
                            new CommandLineOption<int>('f', "From.") { ExclusionGroups = { "EG1" } },
                            new CommandLineOption<int>('t', "To.") { ExclusionGroups = { "EG2" } },
                            new CommandLineSwitch('a', "All.") { ExclusionGroups = { "EG1", "EG2" } },
                            new CommandLineOption<int>('s', "Single.") { ExclusionGroups = { "EG1", "EG2" } }
                        },
                        CommandLineFunc = (o, _) => {
                            theTestOptions = o;
                            return 0;
                        }
                    },
                    new CommandLineSubcommand<object>("build", "Builds something.")
                }
            };

            Assert.Throws<CommandLineError>(() => theCommandLineDefinition.Try(["test"]));
            Assert.That(
                theConsoleOutput.ToString(),
                Is.EqualTo(
                    "Usage: sample test [-f <int>] [-t <int>] [-a] [-s <int>]" + DbLf +
                    "Error: Missing option: -f" + DbLf +
                    "Options" + Lf +
                    "  -f   From." + Lf +
                    "  -t   To." + Lf +
                    "  -a   All." + Lf +
                    "  -s   Single." + DbLf
                )
            );

            theConsoleOutput.GetStringBuilder().Clear();

            theCommandLineDefinition.Try(["test", "-a"]);
            Assert.That(theTestOptions!.All, Is.True);
            Assert.That(theTestOptions.From, Is.EqualTo(0));
            Assert.That(theTestOptions.To, Is.EqualTo(0));
            Assert.That(theTestOptions.Single, Is.EqualTo(0));

            theConsoleOutput.GetStringBuilder().Clear();

            theCommandLineDefinition.Try(["test", "-s", "123"]);
            Assert.That(theTestOptions.All, Is.False);
            Assert.That(theTestOptions.From, Is.EqualTo(0));
            Assert.That(theTestOptions.To, Is.EqualTo(0));
            Assert.That(theTestOptions.Single, Is.EqualTo(123));

            theConsoleOutput.GetStringBuilder().Clear();

            Assert.Throws<CommandLineError>(() => theCommandLineDefinition.Try(["test", "-a", "-s", "123"]));
            Assert.That(
                theConsoleOutput.ToString(),
                Is.EqualTo(
                    "Usage: sample test [-f <int>] [-t <int>] [-a] [-s <int>]" + DbLf +
                    "Error: Only one of the following options may be set: [-a], [-s <int>]" + DbLf +
                    "Options" + Lf +
                    "  -f   From." + Lf +
                    "  -t   To." + Lf +
                    "  -a   All." + Lf +
                    "  -s   Single." + DbLf
                )
            );

            theConsoleOutput.GetStringBuilder().Clear();

            theCommandLineDefinition.Try(["test", "-f", "123", "-t", "456"]);
            Assert.That(theTestOptions.All, Is.False);
            Assert.That(theTestOptions.From, Is.EqualTo(123));
            Assert.That(theTestOptions.To, Is.EqualTo(456));
            Assert.That(theTestOptions.Single, Is.EqualTo(0));

            theConsoleOutput.GetStringBuilder().Clear();

            Assert.Throws<CommandLineError>(() => theCommandLineDefinition.Try(
                ["test", "-f", "123", "-t", "456", "-a"]
            ));
            Assert.That(
                theConsoleOutput.ToString(),
                Is.EqualTo(
                    "Usage: sample test [-f <int>] [-t <int>] [-a] [-s <int>]" + DbLf +
                    "Error: Only one of the following options may be set: [-f <int>], [-a]" + DbLf +
                    "Options" + Lf +
                    "  -f   From." + Lf +
                    "  -t   To." + Lf +
                    "  -a   All." + Lf +
                    "  -s   Single." + DbLf
                )
            );
        }

        [Test]
        public void OptionsAndShowHelp()
        {
            var theConsoleOutput = new StringWriter();

            // decimal als Parametertyp ist beispielsweise nicht erlaubt:
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<TypeInitializationException>(() => new CommandLineOption<decimal>('a', null!));

            BuildOptions? theBuildOptions = null;

            var theCommandLineDefinition = new CommandLineDefinition
            {
                Name = "sample",
                Configuration = new CommandLineConfiguration
                {
                    UsageTextWriter = theConsoleOutput,
                    ErrorAction = () => { }
                },
                Subcommands = {
                    new CommandLineSubcommand<BuildOptions>("build", "Builds something.") {
                        Options = {
                            new CommandLineOption<int>('p', "Sets the project number.") { LongName = "project-number" },
                            new CommandLineOption<string>('m', "Sets the used memory.") {
                                LongName = "memory",
                                DefaultValue = "1 GB"
                            },
                            new CommandLineSwitch('d', "Disables the command.") { ExclusionGroups = { "E1" } },
                            new CommandLineSwitch('z', "Zzzz...") { ExclusionGroups = { "E1" } }
                        },
                        Parameters = {
                            new CommandLineParameter<string>("dummy", "Sets a dummy value.")
                        },
                        CommandLineFunc = (o, _) => {
                            theBuildOptions = o;
                            return 0;
                        }
                    },
                    new CommandLineSubcommand<object>("test", "Tests something.")
                }
            };

            theCommandLineDefinition.ShowHelp();

            Assert.That(
                theConsoleOutput.ToString(),
                Is.EqualTo(
                    "Usage: sample <build | test>" + DbLf +
                    "Subcommands" + Lf +
                    "  build   Builds something." + Lf +
                    "  test    Tests something." + DbLf
                )
            );

            theConsoleOutput.GetStringBuilder().Clear();

            Assert.Throws<ArgumentNullException>(() => theCommandLineDefinition.ShowHelp(null!));
            Assert.Throws<ArgumentException>(() => theCommandLineDefinition.ShowHelp("not-found"));

            theCommandLineDefinition.ShowHelp("build");

            Assert.That(
                theConsoleOutput.ToString(),
                Is.EqualTo(
                    "Usage: sample build [-p, --project-number <int>] [-m, --memory <string=\"1 GB\">]" +
                    " [-d] [-z] <dummy:string>" + DbLf +
                    "Options" + Lf +
                    "  -p, --project-number   Sets the project number." + Lf +
                    "  -m, --memory           Sets the used memory. >>> defaults to \"1 GB\"" + Lf +
                    "  -d                     Disables the command." + Lf +
                    "  -z                     Zzzz..." + DbLf +
                    "Parameters" + Lf +
                    "  dummy                  Sets a dummy value." + DbLf
                )
            );

            theCommandLineDefinition.Configuration.ErrorAction = () => throw new CommandLineError();
            theConsoleOutput.GetStringBuilder().Clear();

            Assert.Throws<CommandLineError>(() => theCommandLineDefinition.Try(["build"]));

            Assert.That(
                theConsoleOutput.ToString(),
                Is.EqualTo(
                    "Usage: sample build [-p, --project-number <int>] [-m, --memory <string=\"1 GB\">]" +
                    " [-d] [-z] <dummy:string>" + DbLf +
                    "Error: Missing option: -p, --project-number" + DbLf +
                    "Options" + Lf +
                    "  -p, --project-number   Sets the project number." + Lf +
                    "  -m, --memory           Sets the used memory. >>> defaults to \"1 GB\"" + Lf +
                    "  -d                     Disables the command." + Lf +
                    "  -z                     Zzzz..." + DbLf +
                    "Parameters" + Lf +
                    "  dummy                  Sets a dummy value." + DbLf
                )
            );

            theConsoleOutput.GetStringBuilder().Clear();

            theCommandLineDefinition.Try(["build", "-p", "0", "dummy"]);
            Assert.That(theBuildOptions!.ProjectNumber, Is.EqualTo(0));
            Assert.That(theBuildOptions.Memory, Is.EqualTo("1 GB"));
            Assert.That(theBuildOptions.DummyParameter, Is.EqualTo("dummy"));
            Assert.That(theBuildOptions.Disable, Is.False);
            Assert.That(theConsoleOutput.ToString(), Is.Empty);

            theCommandLineDefinition.Try(["build", "-d", "-p", "0", "dummy"]);
            Assert.That(theBuildOptions.ProjectNumber, Is.EqualTo(0));
            Assert.That(theBuildOptions.Memory, Is.EqualTo("1 GB"));
            Assert.That(theBuildOptions.DummyParameter, Is.EqualTo("dummy"));
            Assert.That(theBuildOptions.Disable, Is.True);
            Assert.That(theConsoleOutput.ToString(), Is.Empty);

            theCommandLineDefinition.Try(["build", "-d", "-p", "11", "dummy"]);
            Assert.That(theBuildOptions.ProjectNumber, Is.EqualTo(11));
            Assert.That(theBuildOptions.Memory, Is.EqualTo("1 GB"));
            Assert.That(theBuildOptions.DummyParameter, Is.EqualTo("dummy"));
            Assert.That(theBuildOptions.Disable, Is.True);
            Assert.That(theBuildOptions.Zzzz, Is.False);
            Assert.That(theConsoleOutput.ToString(), Is.Empty);

            theCommandLineDefinition.Try(["build", "-d", "--project-number", "99", "-m", "2 GB", "dummy"]);
            Assert.That(theBuildOptions.ProjectNumber, Is.EqualTo(99));
            Assert.That(theBuildOptions.Memory, Is.EqualTo("2 GB"));
            Assert.That(theBuildOptions.DummyParameter, Is.EqualTo("dummy"));
            Assert.That(theBuildOptions.Disable, Is.True);
            Assert.That(theBuildOptions.Zzzz, Is.False);
            Assert.That(theConsoleOutput.ToString(), Is.Empty);

            Assert.Throws<CommandLineError>(() => theCommandLineDefinition.Try(["build", "-dz", "-p", "99", "dummy"]));

            Assert.That(
                theConsoleOutput.ToString(),
                Is.EqualTo(
                    "Usage: sample build [-p, --project-number <int>] [-m, --memory <string=\"1 GB\">]" +
                    " [-d] [-z] <dummy:string>" + DbLf +
                    "Error: Only one of the following options may be set: [-d], [-z]" + DbLf +
                    "Options" + Lf +
                    "  -p, --project-number   Sets the project number." + Lf +
                    "  -m, --memory           Sets the used memory. >>> defaults to \"1 GB\"" + Lf +
                    "  -d                     Disables the command." + Lf +
                    "  -z                     Zzzz..." + DbLf +
                    "Parameters" + Lf +
                    "  dummy                  Sets a dummy value." + DbLf
                )
            );

            theConsoleOutput.GetStringBuilder().Clear();

            Assert.Throws<CommandLineError>(() => theCommandLineDefinition.Try(["build", "-p", "abc", "dummy"]));

            Assert.That(
                theConsoleOutput.ToString(),
                Is.EqualTo(
                    "Usage: sample build [-p, --project-number <int>] [-m, --memory <string=\"1 GB\">]" +
                    " [-d] [-z] <dummy:string>" + DbLf +
                    "Error: Value \"abc\" for option [-p, --project-number <int>] is invalid." + DbLf +
                    "Options" + Lf +
                    "  -p, --project-number   Sets the project number." + Lf +
                    "  -m, --memory           Sets the used memory. >>> defaults to \"1 GB\"" + Lf +
                    "  -d                     Disables the command." + Lf +
                    "  -z                     Zzzz..." + DbLf +
                    "Parameters" + Lf +
                    "  dummy                  Sets a dummy value." + DbLf
                )
            );

            theConsoleOutput.GetStringBuilder().Clear();

            Assert.Throws<CommandLineError>(() => theCommandLineDefinition.Try(["build", "-p"]));

            Assert.That(
                theConsoleOutput.ToString(),
                Does.StartWith(
                    "Usage: sample build [-p, --project-number <int>] [-m, --memory <string=\"1 GB\">]" +
                    " [-d] [-z] <dummy:string>" + DbLf +
                    "Error: Missing value for option [-p, --project-number <int>]."
                )
            );
        }

        private class CommandLineError : Exception { }

        internal class TestOptions
        {
            [CommandLineOption('f')]
            public int From { get; set; }

            [CommandLineOption('t')]
            public int To { get; set; }

            [CommandLineOption('s')]
            public int Single { get; set; }

            [CommandLineOption('a')]
            public bool All { get; set; }
        }

        internal class BuildOptions
        {
            [CommandLineOption('p')]
            public int ProjectNumber { get; set; }

            [CommandLineOption('m')]
            public string? Memory { get; set; }

            [CommandLineOption('d')]
            public bool Disable { get; set; }

            [CommandLineOption('z')]
            public bool Zzzz { get; set; }

            [CommandLineParameter("dummy")]
            public string? DummyParameter { get; set; }
        }
    }
}
