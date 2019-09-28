using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Epos.Utilities
{
    /// <summary>Provides utility methods for pretty-printing arbitrary object instances.</summary>
    public static class DumpExtensions 
    {
        private const string Null = "null";
        private const string My = "my";
        private const string Underscore = "_";
        private const string ToStringMethodName = "ToString";
        private static readonly Type[] ToStringParameters = { typeof(IFormatProvider) };

        /// <summary>Returns a pretty-print string representation of the specified
        /// <paramref name="value"/>.</summary>
        /// <param name="value">Value</param>
        /// <returns>Pretty-print string representation</returns>
        public static string Dump(this object? value) {
            // Simple cases:
            switch (value) {
                case null:
                    return Null;

                case string theString:
                    return theString;

                case double theDoubleValue:
                    return
                        Math.Abs(theDoubleValue) < 1.0E-14 ?
                        "0" :
                        theDoubleValue.ToString("0.##########", CultureInfo.InvariantCulture);

                case DictionaryEntry theEntry:
                    return
                        new StringBuilder("{ ")
                            .Append(theEntry.Key.Dump()).Append(": ")
                            .Append(theEntry.Value.Dump()).Append(" }")
                            .ToString();

                case Type theTypeToDump:
                    return theTypeToDump.Dump();
            }

            Type theType = value!.GetType();
            if (theType.IsGenericType && theType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>)) {
                StringBuilder theBuilder =
                    new StringBuilder("{ ")
                        .Append(theType.GetProperty("Key").GetValue(value, null).Dump())
                        .Append(": ")
                        .Append(theType.GetProperty("Value").GetValue(value, null).Dump())
                        .Append(" }");
				
                return theBuilder.ToString();
            }
			
            // Find ToString() method and call, if available (not with anonymous types!):
            MethodInfo? theToStringMethodInfo = GetToStringMethodInfo(theType);
            if (theToStringMethodInfo != null && !theType.Name.StartsWith("<>f__Anonymous")) {
                if (theToStringMethodInfo.GetParameters().Length == 1) {
                    return (string) theToStringMethodInfo.Invoke(
                        value, new object[] { CultureInfo.InvariantCulture }
                    );
                } else {
                    return (string) theToStringMethodInfo.Invoke(value, null);
                }
            }

            switch (value) {
                case IEnumerable theSequence:
                    return Dump(theSequence.GetEnumerator());

                case IEnumerator theEnumerator:
                    return Dump(theEnumerator);
            }

            // Only object.ToString() respectively ValueType.ToString() is possible.
            // That's not enough.
            FieldInfo[] theFieldInfos = theType.GetFields(
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static
            );

            var theStringBuilder = new StringBuilder("{ ");
            int theFieldInfosLength = theFieldInfos.Length;

            for (int theIndex = 0; theIndex < theFieldInfosLength; theIndex++) {
                FieldInfo theFieldInfo = theFieldInfos[theIndex];
                string theFieldName = theFieldInfo.Name;
				
                if (theFieldName.StartsWith(My, StringComparison.InvariantCulture)) {
                    theFieldName = theFieldName.Substring(2);
                } else if (theFieldName.StartsWith(Underscore, StringComparison.InvariantCulture)) {
                    theFieldName = 
                        theFieldName.Substring(1, 1).ToUpper(CultureInfo.CurrentCulture) + theFieldName.Substring(2);
                } else if (theFieldName.StartsWith("<")) {
                    // Anonymous type fields:
                    theFieldName = theFieldName.Substring(1, theFieldName.IndexOf('>') - 1);
                } else {
                    theFieldName = 
                        theFieldName.Substring(0, 1).ToUpper(CultureInfo.CurrentCulture) + theFieldName.Substring(1);
                }
				
                theStringBuilder.Append(theFieldName).Append(" = ").Append(theFieldInfo.GetValue(value).Dump());
				
                if (theIndex < theFieldInfosLength - 1) {
                    theStringBuilder.Append(", ");
                }
            }

            return theStringBuilder.Append(" }").ToString();
        }

        #region Helper methods

        private static string Dump(this Type type) {
            if (type.IsGenericParameter) {
                return type.Name;
            }
			
            var theResult = new StringBuilder();
            string theFullName;

            bool isNullableType = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

            if (type.IsNested) {
                theResult.Append(type.DeclaringType.Dump()).Append(Type.Delimiter);
                theFullName = type.Name;
            } else {
                theFullName = GetFriendlyName(type);
            }

            int theGenericArgumentIndex = theFullName.IndexOf('`');
            if (theGenericArgumentIndex != -1) {
                theFullName = theFullName.Substring(0, theGenericArgumentIndex);
            }

            if (!isNullableType) {
                theResult.Append(theFullName);
            }

            if (type.IsGenericType) {
                if (!isNullableType) {
                    theResult.Append('<');
                }

                Type[] theTypeArguments = type.GetGenericArguments();
                int theTypeArgumentsLength = theTypeArguments.Length;

                for (int theIndex = 0; theIndex < theTypeArgumentsLength; theIndex++) {
                    Type theTypeArgument = theTypeArguments[theIndex];
                    theResult.Append(theTypeArgument.Dump());
					
                    if (theIndex < theTypeArgumentsLength - 1) {
                        theResult.Append(", ");
                    }
                }

                theResult.Append(!isNullableType ? '>' : '?');
            }

            return theResult.ToString();
        }

        private static string GetFriendlyName(Type type) {
            if (type == typeof(int)) {
                return "int";
            } else if (type == typeof(double)) {
                return "double";
            } else if (type == typeof(bool)) {
                return "bool";
            } else if (type == typeof(byte)) {
                return "byte";
            } else if (type == typeof(char)) {
                return "char";
            } else if (type == typeof(decimal)) {
                return "decimal";
            } else if (type == typeof(short)) {
                return "short";
            } else if (type == typeof(long)) {
                return "long";
            } else if (type == typeof(float)) {
                return "float";
            } else if (type == typeof(void)) {
                return "void";
            } else if (type == typeof(string)) {
                return "string";
            } else if (type == typeof(object)) {
                return "object";
            } else if (type.FullName == "Epos.Core.Date") {
                return "Date";
            } else if (type.FullName == "Epos.Core.DateSpan") {
                return "DateSpan";
            } else if (type.FullName == "Epos.Core.HistoricalState") {
                return "HistoricalState";
            } else if (type.FullName == "Epos.Core.Rational") {
                return "Rational";
            } else {
                return type.FullName;
            }
        }

        private static string Dump(IEnumerator enumerator) {
            StringBuilder theStringBuilder = new StringBuilder().Append('[');
			
            while (enumerator.MoveNext()) {
                object theEntry = enumerator.Current;
				
                theStringBuilder.Append(theEntry.Dump());
                theStringBuilder.Append(", ");
            }

            if (theStringBuilder.Length != 1) {
                theStringBuilder.Length -= 2;
            }
			
            theStringBuilder.Append("]");
			
            return theStringBuilder.ToString();
        }

        private static MethodInfo? GetToStringMethodInfo(Type type) {
            MethodInfo? theResult = null;

            Type theSearchType = type;
            while (theSearchType != typeof(object) &&
                (theResult = theSearchType.GetMethod(
                    ToStringMethodName, 
                    BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance,
                    null, ToStringParameters, null)
                ) == null) {
                theSearchType = theSearchType.BaseType;
            }
            if (theResult != null) {
                return theResult;
            }

            theSearchType = type;
            while ((theResult =
                theSearchType.GetMethod(
                    ToStringMethodName,
                    BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance,
                    null, Type.EmptyTypes, null
                )) == null) {
                theSearchType = theSearchType.BaseType;
            }
            if (theSearchType != typeof(object) && theSearchType != typeof(ValueType)) {
                return theResult;
            }
			
            return null;
        }

        #endregion
    }
}
