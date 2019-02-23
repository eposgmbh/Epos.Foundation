using System;
using System.Collections.Generic;
using System.Text;
using Epos.Utilities;

namespace Epos.CmdLine
{
    /// <summary> Command line option, see <a href="/getting-started.html">Getting started</a>
    /// for an example.</summary>
    /// <typeparam name="T">Option data type</typeparam>
    public class CmdLineOption<T> : CmdLineOption
    {
        private T myDefaultValue;
        private bool myIsDefaultValueSet;

        static CmdLineOption() {
            typeof(T).TestAvailable();
        }

        /// <summary> Initializes an instance of the <see cref="CmdLineOption{T}"/> class.
        /// </summary>
        /// <param name="letter">Option letter</param>
        /// <param name="description">Description</param>
        public CmdLineOption(char letter, string description) : base(typeof(T), letter, description) { }

        /// <summary> Gets or sets the default value that is used, if the option is not
        /// specified on the command line.</summary>
        public T DefaultValue {
            get => myDefaultValue;
            set {
                myDefaultValue = value;
                myIsDefaultValueSet = true;
            }
        }

        internal override object GetDefaultValue() => myIsDefaultValueSet ? (object) DefaultValue : null;
    }

    /// <summary> Command line option base class.</summary>
    public abstract class CmdLineOption
    {
        /// <summary> Initializes an instance of the <see cref="CmdLineOption"/> class.
        /// </summary>
        /// <param name="dataType">Option data type</param>
        /// <param name="letter">Option letter</param>
        /// <param name="description">Description</param>
        protected CmdLineOption(Type dataType, char letter, string description) {
            DataType    = dataType    ?? throw new ArgumentNullException(nameof(dataType));
            Letter      = letter;
            Description = description ?? throw new ArgumentNullException(nameof(description));
        }

        /// <summary> Gets the option letter.</summary>
        public char Letter { get; }

        /// <summary> Gets the description. </summary>
        public string Description { get; }

        /// <summary> Gets the data type. </summary>
        public Type DataType { get; }

        /// <summary> Gets or sets the long name ("--example").
        /// </summary>
        public string LongName { get; set; }

        /// <summary> Exclusion groups to which this option belongs. </summary>
        /// <remarks> Two options with the same exclusion group cannot be used together. </remarks>
        public IList<string> ExclusionGroups { get; } = new List<string>();

        internal bool IsSwitch => DataType == typeof(bool);

        internal string ToShortCmdLineString() {
            string theOptionName = $"-{Letter}";

            if (LongName != null) {
                theOptionName += $", --{LongName}";
            }

            return theOptionName;
        }

        internal string ToLongCmdLineString() {
            StringBuilder theResult = new StringBuilder()
                .Append('[')
                .Append(ToShortCmdLineString());

            if (!IsSwitch) {
                theResult
                    .Append(" <")
                    .Append(DataType.Dump());

                object theDefaultValue = GetDefaultValue();
                if (theDefaultValue != null) {
                    theResult
                        .Append("=");

                    if (theDefaultValue is string) {
                        theResult.Append('"');
                    }

                    theResult.Append(theDefaultValue.Dump());

                    if (theDefaultValue is string) {
                        theResult.Append('"');
                    }
                }

                theResult.Append('>');
            }

            theResult.Append(']');

            return theResult.ToString();
        }

        internal abstract object GetDefaultValue();
    }
}
