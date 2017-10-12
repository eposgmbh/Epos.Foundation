using System;

namespace Epos.CmdLine
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class CmdLineOptionAttribute : Attribute
    {
        public CmdLineOptionAttribute(char letter) {
            Letter = letter;
        }

        public char? Letter { get; }
    }
}
