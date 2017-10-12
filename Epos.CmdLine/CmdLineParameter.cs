using System;
using System.Text;

namespace Epos.CmdLine
{
    public sealed class CmdLineParameter<T> : CmdLineParameter
    {
        private T myDefaultValue;
        private bool myIsDefaultValueSet;

        static CmdLineParameter() {
            typeof(T).TestAvailable();
        }

        public CmdLineParameter(string name, string description) : base(typeof(T), name, description) { }

        public T DefaultValue {
            get => myDefaultValue;
            set {
                myDefaultValue = value;
                myIsDefaultValueSet = true;
            }
        }

        internal override object GetDefaultValue() {
            return myIsDefaultValueSet ? (object)DefaultValue : null;
        }
    }

    public abstract class CmdLineParameter
    {
        protected CmdLineParameter(Type dataType, string name, string description) {
            DataType    = dataType    ?? throw new ArgumentNullException(nameof(dataType));
            Name        = name        ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
        }

        public string Name { get; }

        public string Description { get; }

        public Type DataType { get; }

        public bool IsOptional => GetDefaultValue() != null;

        internal abstract object GetDefaultValue();

        public string ToCmdLineString() {
            var theResult = new StringBuilder();

            if (IsOptional) {
                theResult.Append('[');
            }

            theResult
                .Append('<')
                .Append(Name)
                .Append(":")
                .Append(DataType.GetShortTypeString())
                .Append('>');

            if (IsOptional) {
                theResult.Append("=");

                if (DataType == typeof(string)) {
                    theResult.Append('"');
                }

                theResult.Append(GetDefaultValue());

                if (DataType == typeof(string)) {
                    theResult.Append('"');
                }

                theResult.Append(']');
            }

            return theResult.ToString();
        }
    }
}
