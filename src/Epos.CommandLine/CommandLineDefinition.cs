using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Epos.CommandLine;

/// <summary> Command line definition, see <a href="/getting-started.html">Getting started</a>
/// for an example.</summary>
public sealed class CommandLineDefinition
{
    /// <summary> Initializes an instance of the <see cref="CommandLineDefinition"/> class.
    /// </summary>
    public CommandLineDefinition() {
        Subcommands = new List<CommandLineSubcommand>();
    }

    /// <summary> Gets or sets the name of the command line tool. If not specified
    /// (<b>null</b>) the .exe filename is used.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>Gets or sets whether the definition has differentiated subcommands.
    /// </summary>
    /// <remarks>This should be set to false, if the definition does not need
    /// subcommands.</remarks>
    public bool HasDifferentiatedSubcommands { get; set; } = true;

    /// <summary>Gets or sets the configuration options. You normally should not
    /// change this property value.</summary>
    public CommandLineConfiguration Configuration { get; set; } = new CommandLineConfiguration();

    /// <summary>Gets the list of subcommands.</summary>
    /// <remarks>Here you can register your own subcommands. If you don't need
    /// subcommands, add one subcommand with the name "default".</remarks>
    public IList<CommandLineSubcommand> Subcommands { get; }

    /// <summary>Writes the usage help to the console.</summary>
    public void ShowHelp() {
        var theUsageWriter = new CommandLineUsageWriter(this);
        theUsageWriter.WriteAndExit();
    }

    /// <summary>Writes the usage help for the specified subcommand to
    /// the console.</summary>
    /// <param name="subcommandName">Name of the subcommand</param>
    public void ShowHelp(string subcommandName) {
        if (subcommandName is null) {
            throw new ArgumentNullException(nameof(subcommandName));
        }

        CommandLineSubcommand? theSubcommand =
            Subcommands.SingleOrDefault(sc => sc.Name == subcommandName) ??
            throw new ArgumentException($"Subcommand \"{subcommandName}\" is not defined.");

        var theUsageWriter = new CommandLineUsageWriter(this);
        theUsageWriter.WriteAndExit(theSubcommand);
    }

    /// <summary>Tries to parse the command line and execute the specified
    /// subcommand with the specied options and parameters.</summary>
    /// <param name="args">Command line arguments</param>
    /// <returns>Exit code</returns>
    public int Try(string[] args) {
        if (args is null) {
            throw new ArgumentNullException(nameof(args));
        }
        for (int theIndex = 0; theIndex < args.Length; theIndex++) {
            if (args[theIndex] is null) {
                throw new ArgumentNullException($"args[{theIndex}]");
            }
        }

        if (Name is null) {
            // Standardbelegung aus Dateinamen
            string thePathToExe = Environment.GetCommandLineArgs().First();
            Name = Path.GetFileNameWithoutExtension(thePathToExe).ToLower();
        }

        var theCommandLineDefinitionExecutor = new CommandLineDefinitionExecutor(this, args);

        try {
            return theCommandLineDefinitionExecutor.TryAsync().Result;
        } catch (AggregateException theException) {
            throw theException.InnerException!;
        }
    }

    /// <summary>Tries to parse the command line and execute the specified
    /// subcommand with the specied options and parameters.</summary>
    /// <remarks>Allows for async command line functions
    /// (<see cref="CommandLineSubcommand{TOptions}.AsyncCommandLineFunc"/>).
    /// </remarks>
    /// <param name="args">Command line arguments</param>
    /// <returns>Exit code</returns>
    public async Task<int> TryAsync(string[] args) {
        if (args is null) {
            throw new ArgumentNullException(nameof(args));
        }
        for (int theIndex = 0; theIndex < args.Length; theIndex++) {
            if (args[theIndex] is null) {
                throw new ArgumentNullException($"args[{theIndex}]");
            }
        }

        if (Name is null) {
            // Standardbelegung aus Dateinamen
            string thePathToExe = Environment.GetCommandLineArgs().First();
            Name = Path.GetFileNameWithoutExtension(thePathToExe).ToLower();
        }

        var theCommandLineDefinitionExecutor = new CommandLineDefinitionExecutor(this, args);
        return await theCommandLineDefinitionExecutor.TryAsync();
    }
}
