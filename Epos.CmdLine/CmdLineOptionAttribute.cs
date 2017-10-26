using System;

namespace Epos.CmdLine
{
    /// <summary> Marks an option property on an option class
    /// used in <see cref="CmdLineSubcommand{TOptions}"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class CmdLineOptionAttribute : Attribute
    {
        /// <summary> Initializes an instance of the <see cref="CmdLineOptionAttribute"/>
        /// class associating the marked property with the specified option letter. </summary>
        /// <param name="letter">Option letter</param>
        public CmdLineOptionAttribute(char letter) {
            Letter = letter;
        }

        /// <summary> Gets the option letter. </summary>
        public char Letter { get; }
    }
}
