using System;
using System.Collections.Generic;
using System.Linq;

namespace Epos.CmdLine
{
    internal sealed class CmdLineDefinitionExecutor
    {
        private readonly CmdLineDefinition myDefinition;
        private readonly string[] myArgs;
        private readonly bool myIsVerbose;

        public CmdLineDefinitionExecutor(CmdLineDefinition definition, string[] args, bool isVerbose) {
            myDefinition = definition;
            myArgs = args;
            myIsVerbose = isVerbose;
        }

        public int Try() {
            var theUsageWriter = new CmdLineUsageWriter(myDefinition, myIsVerbose);

            if (myDefinition.HasDifferentiatedCommands && !myDefinition.Subcommands.Any()) {
                throw new InvalidOperationException("At least one subcommand must be added to the definition.");
            }

            if (myDefinition.HasDifferentiatedCommands && myArgs.Length < 1) {
                theUsageWriter.WriteAndExit();
                return -1;
            }

            // Default Command suchen
            CmdLineSubcommand theSubcommand = myDefinition.Subcommands.SingleOrDefault(
                sc => sc.Name == CmdLineSubcommand.DefaultName
            );

            if (!myDefinition.HasDifferentiatedCommands && theSubcommand == null) {
                throw new InvalidOperationException(
                    "Without differentiated commands a subcommand with the name \"default\" must be added to the definition."
                );
            }

            // Token-List erstellen und passenden Subcommand ermitteln (im Fehlerfall wird
            // hier die Usage erklärt und mit ReturnCode -1 der Prozess beendet)
            var theTokenizer = new CmdLineTokenizer(myDefinition, theUsageWriter);
            List<CmdLineToken> theTokens = theTokenizer.Tokenize(myArgs, ref theSubcommand);

            // Subcommand ausführen
            return theSubcommand.Execute(theTokens);
        }
    }
}
