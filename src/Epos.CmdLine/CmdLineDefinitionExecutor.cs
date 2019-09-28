using System;
using System.Collections.Generic;
using System.Linq;

namespace Epos.CmdLine
{
    internal sealed class CmdLineDefinitionExecutor
    {
        private readonly CmdLineDefinition myDefinition;
        private readonly string[] myArgs;

        public CmdLineDefinitionExecutor(CmdLineDefinition definition, string[] args) {
            myDefinition = definition;
            myArgs = args;
        }

        public int Try() {
            var theUsageWriter = new CmdLineUsageWriter(myDefinition);

            if (myDefinition.HasDifferentiatedSubcommands && !myDefinition.Subcommands.Any()) {
                throw new InvalidOperationException("At least one subcommand must be added to the definition.");
            }

            if (myDefinition.HasDifferentiatedSubcommands && myArgs.Length < 1) {
                theUsageWriter.WriteAndExit();
                return -1;
            }

            // Default Command suchen
            CmdLineSubcommand? theSubcommand = myDefinition.Subcommands.SingleOrDefault(
                sc => sc.Name == CmdLineSubcommand.DefaultName
            );

            if (!myDefinition.HasDifferentiatedSubcommands && theSubcommand == null) {
                throw new InvalidOperationException(
                    "Without differentiated commands a subcommand with the name \"default\" must be added to the definition."
                );
            }

            // Token-List erstellen und passenden Subcommand ermitteln (im Fehlerfall wird
            // hier die Usage erklärt und mit ReturnCode -1 der Prozess beendet)
            var theTokenizer = new CmdLineTokenizer(myDefinition, theUsageWriter);
            List<CmdLineToken> theTokens = theTokenizer.Tokenize(myArgs, ref theSubcommand);

            // Subcommand ausführen
            return theSubcommand!.Execute(theTokens, myDefinition);
        }
    }
}
