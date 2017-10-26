using System;
using System.Globalization;
using Epos.Utilities;

namespace Epos.CmdLine
{
    internal static class ParseUtils
    {
        public static object ParseOption(CmdLineOption option, string rawValue, out string errorMessage) {
            Type theDataType = option.DataType;

            if (rawValue.TryConvert(theDataType, CultureInfo.InvariantCulture, out object theConvertedValue)) {
                errorMessage = null;
                return theConvertedValue;
            } else {
                errorMessage = $"Value \"{rawValue}\" for option {option.ToLongCmdLineString()} is invalid.";
                return null;
            }
        }

        public static object ParseParameter(CmdLineParameter parameter, string rawValue, out string errorMessage) {
            Type theDataType = parameter.DataType;

            if (rawValue.TryConvert(theDataType, CultureInfo.InvariantCulture, out object theConvertedValue)) {
                errorMessage = null;
                return theConvertedValue;
            } else {
                errorMessage = $"Value \"{rawValue}\" for parameter {parameter.ToCmdLineString()} is invalid.";
                return null;
            }
        }
    }
}
