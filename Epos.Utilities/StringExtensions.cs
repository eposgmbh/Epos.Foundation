using System;
using System.Text.RegularExpressions;

namespace Epos.Utilities
{
    public static class StringExtensions
    {
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

        public static bool Matches(this string value, string regexPattern) {
            return Regex.IsMatch(value, regexPattern);
        }

        public static bool IsValidEmailAddress(this string value) {
            return Regex.IsMatch(value,
                @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                RegexOptions.IgnoreCase
            );
        }
    }
}
