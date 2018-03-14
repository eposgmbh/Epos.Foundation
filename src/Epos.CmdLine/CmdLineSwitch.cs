namespace Epos.CmdLine
{
    /// <summary> Command line switch, see <a href="/getting-started.html">Getting started</a>
    /// for an example. </summary>
    /// <remarks>A command line switch is a specialized <see cref="CmdLineOption{T}"/> with
    /// type <see cref="bool"/>.</remarks>
    public sealed class CmdLineSwitch : CmdLineOption<bool>
    {
        /// <summary> Initializes an instance of the <see cref="CmdLineSwitch"/> class.
        /// </summary>
        /// <param name="letter">Option letter</param>
        /// <param name="description">Description</param>
        public CmdLineSwitch(char letter, string description) : base(letter, description) { }
    }
}
