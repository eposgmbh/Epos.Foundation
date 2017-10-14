using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Epos.CmdLine
{
    public sealed class CmdLineDefinition
    {
        private CmdLineConfiguration myConfiguration;

        public CmdLineDefinition() {
            Subcommands = new List<CmdLineSubcommand>();
        }

        public string Name { get; set; }

        public bool HasDifferentiatedCommands { get; set; } = true;

        public CmdLineConfiguration Configuration {
            get => myConfiguration ?? (myConfiguration = new CmdLineConfiguration());
            set => myConfiguration = value;
        }

        public IList<CmdLineSubcommand> Subcommands {get; }

        public void ShowHelp() {
            var theUsageWriter = new CmdLineUsageWriter(this);
            theUsageWriter.WriteAndExit();
        }

        public void ShowHelp(string subcommandName) {
            if (subcommandName == null) {
                throw new ArgumentNullException(nameof(subcommandName));
            }

            CmdLineSubcommand theSubcommand = Subcommands.SingleOrDefault(sc => sc.Name == subcommandName);

            if (theSubcommand == null) {
                throw new ArgumentException($"Subcommand \"{subcommandName}\" is not defined.");
            }

            var theUsageWriter = new CmdLineUsageWriter(this);
            theUsageWriter.WriteAndExit(theSubcommand);
        }

        public int Try(string[] args) {
            if (args == null) {
                throw new ArgumentNullException(nameof(args));
            }
            for (int theIndex = 0; theIndex < args.Length; theIndex++) {
                if (args[theIndex] == null) {
                    throw new ArgumentNullException($"args[{theIndex}]");
                }
            }

            if (Name == null) {
                // Standardbelegung aus Dateinamen
                string thePathToExe = Environment.GetCommandLineArgs().First();
                Name = Path.GetFileNameWithoutExtension(thePathToExe).ToLower();
            }
            
            var theCmdLineDefinitionExecutor = new CmdLineDefinitionExecutor(this, args);
            return theCmdLineDefinitionExecutor.Try();
        }
    }
}
