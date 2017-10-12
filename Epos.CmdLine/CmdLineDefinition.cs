using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Epos.CmdLine
{
    public sealed class CmdLineDefinition
    {
        public CmdLineDefinition() {
            Subcommands = new List<CmdLineSubcommand>();
        }

        public string Name { get; set; }

        public bool HasDifferentiatedCommands { get; set; } = true;

        public CmdLineConfiguration Configuration { get; set; }

        public IList<CmdLineSubcommand> Subcommands {get; }

        public int Try(string[] args, bool verbose = true) {
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
            
            var theCmdLineDefinitionExecutor = new CmdLineDefinitionExecutor(this, args, verbose);
            return theCmdLineDefinitionExecutor.Try();
        }
    }
}
