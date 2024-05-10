using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Epos.CommandLine;

/// <summary> Command line definition, see <a href="/getting-started.html">Getting started</a>
/// for an example.</summary>
public sealed class CommandLineDefinition
{
    private CommandLineConfiguration configuration = new CommandLineConfiguration();

    /// <summary> Initializes an instance of the <see cref="CommandLineDefinition"/> class.
    /// </summary>
    public CommandLineDefinition() {
    }

    /// <summary>Gets or sets whether the definition has differentiated subcommands.
    /// </summary>
    /// <remarks>This should be set to false, if the definition does not need
    /// subcommands.</remarks>
    public bool HasDifferentiatedSubcommands { get; set; } = true;

    /// <summary>Gets or sets the configuration options. You normally should not
    /// change this property value.</summary>
    public CommandLineConfiguration Configuration {
        get => configuration;
        set {
            ArgumentNullException.ThrowIfNull(value);
            configuration = value;
        }
    }

    /// <summary>Gets the list of subcommands.</summary>
    /// <remarks>Here you can register your own subcommands. If you don't need
    /// subcommands, add one subcommand with the name "default".</remarks>
    public List<CommandLineSubcommand> Subcommands { get; } = [];

    /// <summary>Writes the usage help to the console.</summary>
    public void ShowHelp() {
        var theUsageWriter = new CommandLineUsageWriter(this);
        theUsageWriter.WriteAndExit();
    }

    /// <summary>Writes the usage help for the specified subcommand to
    /// the console.</summary>
    /// <param name="subcommandName">Name of the subcommand</param>
    public void ShowHelp(string subcommandName) {
        ArgumentNullException.ThrowIfNull(subcommandName);

        CommandLineSubcommand? theSubcommand =
            Subcommands.SingleOrDefault(sc => sc.Name == subcommandName) ??
            throw new ArgumentException($"Subcommand \"{subcommandName}\" is not defined.");

        var theUsageWriter = new CommandLineUsageWriter(this);
        theUsageWriter.WriteAndExit(theSubcommand);
    }

    /// <summary>Tries to parse the command line and execute the specified
    /// subcommand with the specied options and parameters.</summary>
    /// <param name="args">Command line arguments</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Exit code</returns>
    public int Try(string[] args, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(args);
        for (int theIndex = 0; theIndex < args.Length; theIndex++) {
            if (args[theIndex] is null) {
                throw new ArgumentNullException($"args[{theIndex}]");
            }
        }

        SetNameIfNeccessary();

        var theCommandLineDefinitionExecutor = new CommandLineDefinitionExecutor(this, args);

        try {
            return theCommandLineDefinitionExecutor.TryAsync(cancellationToken).Result;
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
    /// <param name="cancellationToken">Cancellation token (optional)</param>
    /// <returns>Exit code</returns>
    public async Task<int> TryAsync(string[] args, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(args);
        for (int theIndex = 0; theIndex < args.Length; theIndex++) {
            if (args[theIndex] is null) {
                throw new ArgumentNullException($"args[{theIndex}]");
            }
        }

        SetNameIfNeccessary();

        var theCommandLineDefinitionExecutor = new CommandLineDefinitionExecutor(this, args);
        return await theCommandLineDefinitionExecutor.TryAsync(cancellationToken);
    }

    #region --- Helper methods ---

    private void SetNameIfNeccessary() {
        if (Configuration.Name is null) {
            // Standardbelegung aus Dateinamen
            string thePathToExe = Environment.GetCommandLineArgs().First();
            Configuration.Name = Path.GetFileNameWithoutExtension(thePathToExe);
        }
    }

    #endregion
}
