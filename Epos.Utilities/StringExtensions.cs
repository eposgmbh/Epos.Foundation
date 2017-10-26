using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Epos.Utilities
{
    /// <summary>Collection of extension methods for the
    /// <see cref="System.String" /> type.</summary>
    public static class StringExtensions
    {
        /// <summary>Extracts a substring between specified delimiter strings.</summary>
        /// <example><code><![CDATA[
        /// var theExtractedString = "Hallo (Welt)!".Extract(between: "(", and: ")"); // Welt
        /// ]]></code></example>
        /// <param name="value">String</param>
        /// <param name="between">Left delimiter</param>
        /// <param name="and">Right delimiter</param>
        /// <returns>Extracted string</returns>
        public static string Extract(this string value, string between, string and) {
            if (value == null) {
                throw new ArgumentNullException(nameof(value));
            }
            if (between == null) {
                throw new ArgumentNullException(nameof(between));
            }
            if (and == null) {
                throw new ArgumentNullException(nameof(and));
            }

            int theLeftIndex = value.IndexOf(between, StringComparison.Ordinal);
            if (theLeftIndex == -1) {
                return string.Empty;
            }

            int theRightIndex = value.IndexOf(and, StringComparison.Ordinal);
            if (theRightIndex == -1) {
                return value.Substring(theLeftIndex + 1);
            }

            if (theRightIndex < theLeftIndex) {
                return string.Empty;
            }

            return value.Substring(theLeftIndex + between.Length, theRightIndex - theLeftIndex - and.Length);
        }

        /// <summary>Determines whether the specified string matches the specified
        /// <paramref name="regexPattern"/>.</summary>
        /// <param name="value">String</param>
        /// <param name="regexPattern">Regex pattern</param>
        /// <returns><b>true</b>, if the pattern matches the string; otherwise <b>false</b></returns>
        public static bool Matches(this string value, string regexPattern) {
            return Regex.IsMatch(value, regexPattern);
        }

        /// <summary>Determines whether the specified string is a valid email address.</summary>
        /// <param name="value">String</param>
        /// <returns><b>true</b>, if the specified string is a valid email address;
        /// otherwise <b>false</b></returns>
        public static bool IsValidEmailAddress(this string value) {
            return Regex.IsMatch(value,
                @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                RegexOptions.IgnoreCase
            );
        }

        /// <summary> Tries to convert a string into an instance of <paramref name="destinationType"/>.
        /// </summary>
        /// <remarks>The <see cref="System.Globalization.CultureInfo.CurrentCulture"/> is used for
        /// conversion.</remarks>
        /// <param name="value">String</param>
        /// <param name="destinationType">Destination type</param>
        /// <param name="convertedValue">Converted value result</param>
        /// <returns><b>true</b>, if the conversion succeeds; otherwise <b>false</b></returns>
        public static bool TryConvert(this string value, Type destinationType, out object convertedValue) {
            return TryConvert(value, destinationType, CultureInfo.CurrentCulture, out convertedValue);
        }

        /// <summary> Tries to convert a string into an instance of <paramref name="destinationType"/>
        /// with the specified <paramref name="cultureInfo"/>.</summary>
        /// <param name="value">String</param>
        /// <param name="destinationType">Destination type</param>
        /// <param name="cultureInfo">Culture info</param>
        /// <param name="convertedValue">Converted value result</param>
        /// <returns><b>true</b>, if the conversion succeeds; otherwise <b>false</b></returns>
        public static bool TryConvert(this string value, Type destinationType, CultureInfo cultureInfo,
            out object convertedValue) {
            try {
                bool isNullableType = destinationType.IsGenericType &&
                                      destinationType.GetGenericTypeDefinition() == typeof(Nullable<>);
                Type theDestinationType =
                    isNullableType ? destinationType.GetGenericArguments().Single() : destinationType;

                TypeConverter theConverter = TypeDescriptor.GetConverter(theDestinationType);
                if (!theConverter.CanConvertFrom(typeof(string))) {
                    convertedValue = null;
                    return false;
                }

                object theValue = theConverter.ConvertFromString(null, cultureInfo, value);
                convertedValue = isNullableType ? Activator.CreateInstance(destinationType, theValue) : theValue;

                return true;
            } catch (Exception) {
                convertedValue = null;
                return false;
            }
        }

        /// <summary> Tries to convert a string into an instance of <typeparamref name="TDestinationType"/>.
        /// </summary>
        /// <remarks>The <see cref="System.Globalization.CultureInfo.CurrentCulture"/> is used for
        /// conversion.</remarks>
        /// <typeparam name="TDestinationType">Destination type</typeparam>
        /// <param name="value">String</param>
        /// <param name="convertedValue">Converted value result</param>
        /// <returns><b>true</b>, if the conversion succeeds; otherwise <b>false</b></returns>
        public static bool TryConvert<TDestinationType>(this string value, out TDestinationType convertedValue) {
            return TryConvert(value, CultureInfo.CurrentCulture, out convertedValue);
        }

        /// <summary> Tries to convert a string into an instance of <typeparamref name="TDestinationType"/>.
        /// with the specified <paramref name="cultureInfo"/>.</summary>
        /// <typeparam name="TDestinationType">Destination type</typeparam>
        /// <param name="value">String</param>
        /// <param name="cultureInfo">Culture info</param>
        /// <param name="convertedValue">Converted value result</param>
        /// <returns><b>true</b>, if the conversion succeeds; otherwise <b>false</b></returns>
        public static bool TryConvert<TDestinationType>(this string value, CultureInfo cultureInfo,
            out TDestinationType convertedValue) {
            bool theResult = TryConvert(value, typeof(TDestinationType), cultureInfo, out object theConvertedValue);

            if (theResult) {
                convertedValue = (TDestinationType) theConvertedValue;
            } else {
                convertedValue = default(TDestinationType);
            }

            return theResult;
        }
    }
}
