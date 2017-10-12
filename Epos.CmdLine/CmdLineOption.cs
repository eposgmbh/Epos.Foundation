using System;
using System.Text;

namespace Epos.CmdLine
{
    public class CmdLineOption<T> : CmdLineOption
    {
        private T myDefaultValue;
        private bool myIsDefaultValueSet;

        static CmdLineOption() {
            typeof(T).TestAvailable();
        }

        public CmdLineOption(char letter, string description) : base(typeof(T), letter, description) { }

        public T DefaultValue {
            get => myDefaultValue;
            set {
                myDefaultValue = value;
                myIsDefaultValueSet = true;
            }
        }

        internal override object GetDefaultValue() => myIsDefaultValueSet ? (object) DefaultValue : null;
    }

    public abstract class CmdLineOption
    {
        protected CmdLineOption(Type dataType, char letter, string description) {
            DataType    = dataType    ?? throw new ArgumentNullException(nameof(dataType));
            Letter      = letter;
            Description = description ?? throw new ArgumentNullException(nameof(description));
        }

        public char Letter { get; }

        public string Description { get; }

        public Type DataType { get; }

        public string LongName { get; set; }

        public bool IsSwitch => DataType == typeof(bool);

        public override string ToString() {
            string theOptionName = $"-{Letter}";

            if (LongName != null) {
                theOptionName += $", --{LongName}";
            }

            return theOptionName;
        }

        public string ToCmdLineString() {
            var theResult = new StringBuilder()
                .Append('[')
                .Append(this);

            if (!IsSwitch) {
                theResult
                    .Append(" <")
                    .Append(DataType.GetShortTypeString());

                object theDefaultValue = GetDefaultValue();
                if (theDefaultValue != null) {
                    theResult
                        .Append("=");

                    if (theDefaultValue is string) {
                        theResult.Append('"');
                    }

                    theResult.Append(theDefaultValue);

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
