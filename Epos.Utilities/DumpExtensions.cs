using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Epos.Utilities
{
    /// <summary> Bietet Methoden zur Ausgabe einer String-Repräsentation aller Datentypen </summary>
    public static class DumpExtensions 
    {
        // --- Konstanten ---

        private const string Null = "Null";
        private const string My = "my";
        private const string Underscore = "_";
        private const string ToStringMethodName = "ToString";
        private static readonly Type[] ToStringParameters = { typeof(IFormatProvider) };

        // --- Methoden ---

        public static string Dump(this object value) {
            if (value == null) {
                return Null;
            }

            if (value is string theString) {
                return theString;
            }

            if (value is double theDoubleValue) {
                if (Math.Abs(theDoubleValue) < 1.0E-14) {
                    return "0";
                }

                return theDoubleValue.ToString("0.##########", CultureInfo.InvariantCulture);
            }

            if (value is DictionaryEntry theEntry) {
                var theBuilder = new StringBuilder("[");
                theBuilder.Append(theEntry.Key.Dump()).Append(", ");
                theBuilder.Append(theEntry.Value.Dump()).Append("]");
				
                return theBuilder.ToString();
            }

            if (value is Type theType) {
                return theType.Dump();
            }

            theType = value.GetType();
            if (theType.IsGenericType && theType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>)) {
                var theBuilder = new StringBuilder("[");
                theBuilder.Append(theType.GetProperty("Key").GetValue(value, null).Dump()).Append(", ");
                theBuilder.Append(theType.GetProperty("Value").GetValue(value, null).Dump()).Append("]");
				
                return theBuilder.ToString();
            }
			
            // ToString()-Methode suchen und aufrufen, falls vorhanden (nicht bei Anonymous types!):
            MethodInfo theToStringMethodInfo = GetToStringMethodInfo(theType);
            if (theToStringMethodInfo != null && !theType.Name.StartsWith("<>f__Anonymous")) {
                if (theToStringMethodInfo.GetParameters().Length == 1) {
                    return theToStringMethodInfo.Invoke(
                        value, new object[] { CultureInfo.InvariantCulture }
                    ) as string;
                } else {
                    return theToStringMethodInfo.Invoke(value, null) as string;
                }
            }

            if (value is IEnumerable theSequence) {
                return Dump(theSequence.GetEnumerator());
            }

            if (value is IEnumerator theEnumerator) {
                return Dump(theEnumerator);
            }

            // Nur object.ToString() bzw. ValueType.ToString() ist möglich.
            // Dies reicht nicht aus.
            FieldInfo[] theFieldInfos = theType.GetFields(
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static
            );
            var theStringBuilder = new StringBuilder("[");

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

            return theStringBuilder.Append(']').ToString();
        }

        #region Hilfsmethoden

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
            StringBuilder theStringBuilder = new StringBuilder().Append('{');
			
            while (enumerator.MoveNext()) {
                object theEntry = enumerator.Current;
				
                theStringBuilder.Append(theEntry.Dump());
                theStringBuilder.Append(", ");
            }

            if (theStringBuilder.Length != 1) {
                theStringBuilder.Length = theStringBuilder.Length - 2;
            }
			
            theStringBuilder.Append("}");
			
            return theStringBuilder.ToString();
        }

        private static MethodInfo GetToStringMethodInfo(Type type) {
            MethodInfo theResult = null;

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