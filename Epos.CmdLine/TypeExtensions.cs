using System;
using System.Collections.Generic;
using System.Linq;

namespace Epos.CmdLine
{
    internal static class TypeExtensions
    {
        private static readonly HashSet<Type> AllowedTypes = new HashSet<Type> {
            typeof(bool),
            typeof(int),
            typeof(string),
            typeof(double),
            typeof(DateTime)
        };

        public static void TestAvailable(this Type dataType) {
            if (!AllowedTypes.Contains(dataType)) {
                throw new InvalidOperationException(
                    "DataType " + dataType.FullName + " is not allowed for options or parameters. Allowed types: " +
                    string.Join(", ", AllowedTypes.Select(t => t.Name))
                );
            }
        }

        public static string GetShortTypeString(this Type dataType) {
            if (dataType == typeof(bool)) {
                return "bool";
            }
            else if (dataType == typeof(int)) {
                return "int";
            }
            else if (dataType == typeof(string)) {
                return "string";
            }
            else if (dataType == typeof(double)) {
                return "double";
            }
            else {
                // DateTime
                return "datetime";
            }
        }
    }
}
