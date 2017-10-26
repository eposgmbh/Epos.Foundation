using System;
using System.Text;
using Epos.Utilities;

namespace Epos.CmdLine
{

    /// <summary> Command line parameter, see <a href="/getting-started.html">Getting started</a>
    /// for an example.</summary>
    /// <typeparam name="T">Parameter data type</typeparam>
    public sealed class CmdLineParameter<T> : CmdLineParameter
    {
        private T myDefaultValue;
        private bool myIsDefaultValueSet;

        static CmdLineParameter() {
            typeof(T).TestAvailable();
        }

        /// <summary> Initializes an instance of the <see cref="CmdLineParameter{T}"/> class.
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <param name="description">Description</param>
        public CmdLineParameter(string name, string description) : base(typeof(T), name, description) { }

        /// <summary> Gets or sets the default value that is used, if the parameter is not
        /// specified on the command line.</summary>
        /// <remarks> Specifiying a default value makes the parameter an optional parameter. </remarks>
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

    /// <summary> Command line parameter base class.</summary>
    public abstract class CmdLineParameter
    {
        /// <summary> Initializes an instance of the <see cref="CmdLineParameter"/> class.
        /// </summary>
        /// <param name="dataType">Parameter data type</param>
        /// <param name="name">Parameter name</param>
        /// <param name="description">Description</param>
        protected CmdLineParameter(Type dataType, string name, string description) {
            DataType    = dataType    ?? throw new ArgumentNullException(nameof(dataType));
            Name        = name        ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
        }

        /// <summary> Gets the parameter name.</summary>
        public string Name { get; }

        /// <summary> Gets the description. </summary>
        public string Description { get; }

        /// <summary> Gets the data type. </summary>
        public Type DataType { get; }

        /// <summary> Determines whether the parameter is optional or not.
        /// </summary>
        /// <remarks> A specified <see cref="CmdLineParameter{T}.DefaultValue"/> makes
        /// the parameter optional. </remarks>
        public bool IsOptional => GetDefaultValue() != null;

        internal abstract object GetDefaultValue();

        internal string ToCmdLineString() {
            var theResult = new StringBuilder();

            if (IsOptional) {
                theResult.Append('[');
            }

            theResult
                .Append('<')
                .Append(Name)
                .Append(":")
                .Append(DataType.Dump())
                .Append('>');

            if (IsOptional) {
                theResult.Append("=");

                if (DataType == typeof(string)) {
                    theResult.Append('"');
                }

                theResult.Append(GetDefaultValue().Dump());

                if (DataType == typeof(string)) {
                    theResult.Append('"');
                }

                theResult.Append(']');
            }

            return theResult.ToString();
        }
    }
}
