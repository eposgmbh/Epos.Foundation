using System;
using System.Globalization;

using Epos.Utilities;

namespace Epos.CommandLine;

internal static class ParseUtils
{
    public static object? ParseOption(CommandLineOption option, string rawValue, out string? errorMessage)
    {
        Type theDataType = option.DataType;

        if (theDataType == typeof(System.IO.FileInfo)) {
            errorMessage = null;
            return new System.IO.FileInfo(rawValue);
        }

        if (theDataType == typeof(System.IO.DirectoryInfo)) {
            errorMessage = null;
            return new System.IO.DirectoryInfo(rawValue);
        }

        if (theDataType == typeof(System.Uri)) {
            if (Uri.TryCreate(rawValue, UriKind.Absolute, out Uri? theUri)) {
                errorMessage = null;
                return theUri;
            }
            else
            {
                errorMessage = $"Value \"{rawValue}\" for option {option.ToLongCommandLineString()} is not a valid URI.";
                return null;
            }
        }

        if (rawValue.TryConvert(theDataType, CultureInfo.InvariantCulture, out object? theConvertedValue))
        {
            errorMessage = null;
            return theConvertedValue;
        }
        else
        {
            errorMessage = $"Value \"{rawValue}\" for option {option.ToLongCommandLineString()} is invalid.";
            return null;
        }
    }

    public static object? ParseParameter(CommandLineParameter parameter, string rawValue, out string? errorMessage)
    {
        Type theDataType = parameter.DataType;

        if (theDataType == typeof(System.IO.FileInfo)) {
            errorMessage = null;
            return new System.IO.FileInfo(rawValue);
        }

        if (theDataType == typeof(System.IO.DirectoryInfo)) {
            errorMessage = null;
            return new System.IO.DirectoryInfo(rawValue);
        }

        if (rawValue.TryConvert(theDataType, CultureInfo.InvariantCulture, out object? theConvertedValue))
        {
            errorMessage = null;
            return theConvertedValue;
        }
        else
        {
            errorMessage = $"Value \"{rawValue}\" for parameter {parameter.ToCommandLineString()} is invalid.";
            return null;
        }
    }
}
