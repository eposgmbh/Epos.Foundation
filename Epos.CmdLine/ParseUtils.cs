using System;

namespace Epos.CmdLine
{
    internal static class ParseUtils
    {
        public static object ParseOption(CmdLineOption option, string rawValue, out string errorMessage) {
            Type theDataType = option.DataType;

            try {
                errorMessage = null;
                return ParseCore(theDataType, rawValue);
            }
            catch {
                errorMessage = $"Value \"{rawValue}\" for option {option.ToCmdLineString()} is invalid.";
                return null;
            }
        }

        public static object ParseParameter(CmdLineParameter parameter, string rawValue, out string errorMessage) {
            Type theDataType = parameter.DataType;

            try {
                errorMessage = null;
                return ParseCore(theDataType, rawValue);
            }
            catch {
                errorMessage = $"Value \"{rawValue}\" for parameter {parameter.ToCmdLineString()} is invalid.";
                return null;
            }
        }

        private static object ParseCore(Type dataType, string rawValue) {
            if (dataType == typeof(bool)) {
                return bool.Parse(rawValue);
            }
            else if (dataType == typeof(int)) {
                return int.Parse(rawValue);
            }
            else if (dataType == typeof(string)) {
                return rawValue;
            }
            else if (dataType == typeof(double)) {
                return double.Parse(rawValue);
            }
            else {
                // DateTime
                return DateTime.Parse(rawValue);
            }
        }
    }
}
