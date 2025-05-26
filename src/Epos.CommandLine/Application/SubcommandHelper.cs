namespace Epos.CommandLine.Application;

internal class SubcommandHelper(CommandLineDefinition commandLineDefinition) : ISubcommandHelper
{
    public void ShowHelp() {
        commandLineDefinition.ShowHelp();
    }
}
