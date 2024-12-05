using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Epos.CommandLine.Application;

/// <summary> Interface for subcommands. </summary>
public interface ISubcommand
{
    /// <summary> Default name for the single subcommand, if no
    /// differentiated subcommands are to be used. </summary>
    const string DefaultName = "default";

    /// <summary> Gets the name of the subcommand. </summary>
    /// <remarks> If you only register a single subcommand with the name 'default', the CLI
    /// will not have subcommands. </remarks>
    string Name => DefaultName;

    /// <summary> Gets the description of the subcommand. </summary>
    string Description => "Missing description!";

    /// <summary> Sub command handler method </summary>
    /// <remarks> The subcommand handler method is called on an
    /// instance that is initialized with the command line arguments. The method is
    /// only called, if the command line can be parsed successfully. The method should
    /// return an error code (0 if successful). </remarks>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Exit code (async)</returns>
    public Task<int> ExecuteAsync(CancellationToken cancellationToken);
}
