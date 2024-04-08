namespace Epos.CommandLine;

/// <summary> Command line switch, see <a href="/getting-started.html">Getting started</a>
/// for an example. </summary>
/// <remarks>A command line switch is a specialized <see cref="CommandLineOption{T}"/> with
/// type <see cref="bool"/>.</remarks>
public sealed class CommandLineSwitch : CommandLineOption<bool>
{
    /// <summary> Initializes an instance of the <see cref="CommandLineSwitch"/> class.
    /// </summary>
    /// <param name="letter">Option letter</param>
    /// <param name="description">Description</param>
    public CommandLineSwitch(char letter, string description) : base(letter, description) { }
}
