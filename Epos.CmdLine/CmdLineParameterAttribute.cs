using System;

namespace Epos.CmdLine
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class CmdLineParameterAttribute : Attribute
    {
        public CmdLineParameterAttribute() {
            Name = null;
        }

        public CmdLineParameterAttribute(string name) {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public string Name { get; }
    }
}
