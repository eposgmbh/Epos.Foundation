using System;

namespace Epos.CmdLine
{
    /// <summary> Marks a parameter property on an option class
    /// used in <see cref="CmdLineSubcommand{TOptions}"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class CmdLineParameterAttribute : Attribute
    {
        /// <summary> Initializes an instance of the <see cref="CmdLineParameterAttribute"/>
        /// class associating the marked property with the specified parameter name. </summary>
        /// <param name="name">Parameter name</param>
        public CmdLineParameterAttribute(string name) {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        /// <summary> Gets the parameter name. </summary>
        public string Name { get; }
    }
}
