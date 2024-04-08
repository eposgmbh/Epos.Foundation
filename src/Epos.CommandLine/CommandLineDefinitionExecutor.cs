using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epos.CommandLine;

internal sealed class CommandLineDefinitionExecutor
{
    private readonly CommandLineDefinition myDefinition;
    private readonly string[] myArgs;

    public CommandLineDefinitionExecutor(CommandLineDefinition definition, string[] args) {
        myDefinition = definition;
        myArgs = args;
    }

    public async Task<int> TryAsync() {
        var theUsageWriter = new CommandLineUsageWriter(myDefinition);

        if (myDefinition.HasDifferentiatedSubcommands && !myDefinition.Subcommands.Any()) {
            throw new InvalidOperationException("At least one subcommand must be added to the definition.");
        }

        if (myDefinition.HasDifferentiatedSubcommands && myArgs.Length < 1) {
            theUsageWriter.WriteAndExit();
            return -1;
        }

        // Default Command suchen
        CommandLineSubcommand? theSubcommand = myDefinition.Subcommands.SingleOrDefault(
            sc => sc.Name == CommandLineSubcommand.DefaultName
        );

        if (!myDefinition.HasDifferentiatedSubcommands && theSubcommand is null) {
            throw new InvalidOperationException(
                "Without differentiated commands a subcommand with the name \"default\" must be added to the definition."
            );
        }

        // Token-List erstellen und passenden Subcommand ermitteln (im Fehlerfall wird
        // hier die Usage erklärt und mit ReturnCode -1 der Prozess beendet)
        var theTokenizer = new CommandLineTokenizer(myDefinition, theUsageWriter);
        List<CommandLineToken> theTokens = theTokenizer.Tokenize(myArgs, ref theSubcommand);

        // Subcommand ausführen
        return await theSubcommand!.ExecuteAsync(theTokens, myDefinition);
    }
}
