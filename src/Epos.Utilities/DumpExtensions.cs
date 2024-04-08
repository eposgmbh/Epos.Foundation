using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

using static Epos.Utilities.Characters;

namespace Epos.Utilities;

/// <summary>Provides utility methods for pretty-printing arbitrary object instances.</summary>
public static class DumpExtensions
{
    private const string Null = "null";
    private const string My = "my";
    private const string Underscore = "_";
    private const string ToStringMethodName = "ToString";
    private static readonly Type[] ToStringParameters = [typeof(IFormatProvider)];

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

        Type theType = value.GetType();
        if (theType.IsGenericType && theType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>)) {
            StringBuilder theBuilder =
                new StringBuilder("{ ")
                    .Append(theType.GetProperty("Key")!.GetValue(value, null).Dump())
                    .Append(": ")
                    .Append(theType.GetProperty("Value")!.GetValue(value, null).Dump())
                    .Append(" }");

            return theBuilder.ToString();
        }

        // Find ToString() method and call, if available (not with anonymous types!):
        MethodInfo? theToStringMethodInfo = GetToStringMethodInfo(theType);
        if (theToStringMethodInfo is not null && !theType.Name.StartsWith("<>f__Anonymous")) {
            if (theToStringMethodInfo.GetParameters().Length == 1) {
                return (string) theToStringMethodInfo.Invoke(value, [CultureInfo.InvariantCulture])!;
            } else {
                return (string) theToStringMethodInfo.Invoke(value, null)!;
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

    /// <summary>Returns a pretty-print string representation of the specified
    /// <paramref name="table"/> object.</summary>
    /// <param name="table">Table object</param>
    /// <param name="tableInfoProvider">Table info provider</param>
    /// <returns>Pretty-print string representation of the table</returns>
    public static string DumpTableObject<T, TColumn, TRow>(
        this T table, TableInfoProvider<TColumn, TRow> tableInfoProvider
    ) where T : class {
        if (table is null) {
            throw new ArgumentNullException(nameof(table));
        }
        if (tableInfoProvider is null) {
            throw new ArgumentNullException(nameof(tableInfoProvider));
        }

        var theResult = new StringBuilder();

        List<TextColumn<TColumn>> theColumns = GetTextColumns(tableInfoProvider);

        string theSpaces = new string(' ', tableInfoProvider.LeadingSpaces);

        theResult.Append(theSpaces);

        int theColumnCount = theColumns.Count;

        if (theColumns.Any(c => c.Seperator is not null)) {
            for (int theIndex = 0; theIndex < theColumnCount; theIndex++) {
                TextColumn<TColumn> theColumn = theColumns[theIndex];

                ColumnSeperator? theSeperator = theColumn.Seperator;
                if (theSeperator is not null) {
                    int theColSpan = theSeperator.ColSpan;

                    int theWidth;
                    if (theColSpan == 1) {
                        theWidth = theColumn.Width;
                    } else {
                        theWidth =
                            theColumns.Skip(theIndex).Take(theColSpan).Sum(c => c.Width) + ((theColSpan - 1) * 3);
                    }

                    var theCenteredHeader = new StringBuilder(theSeperator.Header);
                    while (theCenteredHeader.Length < theWidth) {
                        if (theCenteredHeader.Length % 2 == 0) {
                            theCenteredHeader.Insert(0, ' ');
                        } else {
                            theCenteredHeader.Append(' ');
                        }
                    }

                    theResult.Append(theCenteredHeader);

                    theIndex += theColSpan - 1;
                }

                if (theIndex < theColumnCount - 1) {
                    theResult.Append(" | ");
                }
            }

            theResult
                .Append(Lf)
                .Append(theSpaces);
        }

        for (int theIndex = 0; theIndex < theColumnCount; theIndex++) {
            TextColumn<TColumn> theColumn = theColumns[theIndex];
            theResult.Append(string.Format(GetAlignmentString(theColumn), theColumn.Header));

            if (theIndex < theColumnCount - 1) {
                theResult.Append(" | ");
            }
        }

        if (theColumns.Any(c => c.SecondaryHeader is not null)) {
            theResult
                .Append(Lf)
                .Append(theSpaces);

            for (int theIndex = 0; theIndex < theColumnCount; theIndex++) {
                TextColumn<TColumn> theColumn = theColumns[theIndex];

                theResult.Append(string.Format(GetAlignmentString(theColumn), theColumn.SecondaryHeader));

                if (theIndex < theColumnCount - 1) {
                    theResult.Append(" | ");
                }
            }
        }

        if (theColumns.Any(c => c.DataType is not null)) {
            theResult
              .Append(Lf)
                .Append(theSpaces);

            for (int theIndex = 0; theIndex < theColumnCount; theIndex++) {
                TextColumn<TColumn> theColumn = theColumns[theIndex];

                theResult.Append(string.Format(GetAlignmentString(theColumn), theColumn.DataType));

                if (theIndex < theColumnCount - 1) {
                    theResult.Append(" | ");
                }
            }
        }

        theResult.Append(Lf);

        var theSeperatorLine = new StringBuilder();
        for (int theIndex = 0; theIndex < theColumnCount; theIndex++) {
            theSeperatorLine.Append('-', theColumns[theIndex].Width);
            if (theIndex < theColumnCount - 1) {
                theSeperatorLine.Append("-|-");
            }
        }

        theResult
            .Append(theSpaces)
            .Append(theSeperatorLine)
            .Append(Lf);

        int theRowCount = theColumns.First().Rows.Count;

        string? theEmptyTableText = tableInfoProvider.EmptyTableText;

        if (theEmptyTableText is not null && theRowCount == 0) {
            theResult
                .Append(theSpaces)
                .Append(theEmptyTableText)
                .Append(Lf);
        } else {
            for (int theRowIndex = 0; theRowIndex < theRowCount; theRowIndex++) {
                theResult.Append(theSpaces);

                if (theRowIndex == theRowCount - 1) {
                    // Letzte Zeile
                    if (tableInfoProvider.HasSumRow) {
                        theResult
                            .Append(theSeperatorLine)
                            .Append(Lf)
                            .Append(theSpaces);
                    }
                }

                for (int theIndex = 0; theIndex < theColumnCount; theIndex++) {
                    TextColumn<TColumn> theColumn = theColumns[theIndex];

                    (string theCellValue, int theColSpan) = theColumn.Rows[theRowIndex];

                    string theAlignmentString;
                    if (theColSpan == 1) {
                        theAlignmentString = GetAlignmentString(theColumn);
                    } else {
                        int theWidth =
                            theColumns.Skip(theIndex).Take(theColSpan).Sum(c => c.Width) + ((theColSpan - 1) * 3);

                        theAlignmentString = $"{{0,-{theWidth}}}";
                    }

                    theResult.Append(string.Format(theAlignmentString, theCellValue));

                    theIndex += theColSpan - 1;

                    if (theIndex < theColumnCount - 1) {
                        theResult.Append(" | ");
                    }
                }

                theResult.Append(Lf);
            }
        }

        if (tableInfoProvider.HasTrailingSeperatorLine && (theRowCount != 0 || theEmptyTableText is not null)) {
            theResult
                .Append(theSpaces)
                .Append(theSeperatorLine)
                .Append(Lf);
        }

        return theResult.ToString();
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
            theResult.Append(type.DeclaringType!.Dump()).Append(Type.Delimiter);
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
            return type.FullName!;
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

        theStringBuilder.Append(']');

        return theStringBuilder.ToString();
    }

    private static MethodInfo? GetToStringMethodInfo(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] Type type
    ) {
        MethodInfo? theResult = null;

        Type theSearchType = type;
        while (theSearchType != typeof(object) &&
            (theResult = theSearchType!.GetMethod(
                ToStringMethodName,
                BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance,
                null, ToStringParameters, null)
            ) is null) {
            theSearchType = theSearchType.BaseType!;
        }

        if (theResult is not null) {
            return theResult;
        }

        theSearchType = type;
        while ((theResult =
            theSearchType!.GetMethod(
                ToStringMethodName,
                BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance,
                null, Type.EmptyTypes, null
            )) is null) {
            theSearchType = theSearchType.BaseType!;
        }
        if (theSearchType != typeof(object) && theSearchType != typeof(ValueType)) {
            return theResult;
        }

        return null;
    }

    private static string GetAlignmentString<TColumn>(TextColumn<TColumn> column)
        => column.AlignRight ? $"{{0,{column.Width}}}" : $"{{0,-{column.Width}}}";

    private static List<TextColumn<TColumn>> GetTextColumns<TColumn, TRow>(
        TableInfoProvider<TColumn, TRow> tableInfoProvider
    ) {
        var theResult = new List<TextColumn<TColumn>>();

        IEnumerable<TColumn> theColumns = tableInfoProvider.GetColumns();

        int theColumnIndex = 0;
        foreach (TColumn theColumn in theColumns) {
            ColumnInfo theColumnInfo = tableInfoProvider.GetColumnInfo(theColumn, theColumnIndex);

            theResult.Add(
                new TextColumn<TColumn>(theColumnInfo.Header, theColumnIndex, theColumn, theColumnInfo.AlignRight) {
                    SecondaryHeader = theColumnInfo.SecondaryHeader,
                    DataType = theColumnInfo.DataType,
                    Seperator = theColumnInfo.Seperator
                }
            );

            if ((theColumnInfo.Seperator?.ColSpan ?? int.MaxValue) < 1) {
                throw new InvalidOperationException("ColSpan must not be lower than 1.");
            }

            theColumnIndex++;
        }

        IEnumerable<TRow> theRows = tableInfoProvider.GetRows();

        foreach (TRow theRow in theRows) {
            int theColumnCount = theResult.Count;

            theColumnIndex = 0;
            for (int theIndex = 0; theIndex < theColumnCount; theIndex++) {
                TextColumn<TColumn> theTextColumn = theResult[theColumnIndex];

                (string CellValue, int ColSpan) theValue =
                    tableInfoProvider.GetCellValue(theRow, theTextColumn.Column, theIndex);

                if (theValue.ColSpan < 1) {
                    throw new InvalidOperationException("ColSpan must not be lower than 1.");
                } else if (theValue.ColSpan > 1) {
                    int theRemainingColumnCount = theColumnCount - theIndex;

                    if (theValue.ColSpan > theRemainingColumnCount) {
                        throw new InvalidOperationException(
                            $"ColSpan must not be greater than than the remaining column " +
                            $"count ({theRemainingColumnCount})."
                        );
                    }
                }

                theTextColumn.Rows.Add(theValue);

                if (theValue.ColSpan == 1) {
                    theTextColumn.Width = Math.Max(theTextColumn.Width, theValue.CellValue.Length);
                } else {
                    int theTotalWidth = theTextColumn.Width;
                    for (int theNextIndex = theIndex + 1; theNextIndex < theIndex + theValue.ColSpan; theNextIndex++) {
                        TextColumn<TColumn> theNextTextColumn = theResult[theNextIndex];

                        theNextTextColumn.Rows.Add((string.Empty, 1));
                        theTotalWidth += theNextTextColumn.Width;
                    }

                    while (theTotalWidth < theValue.CellValue.Length) {
                        for (int theNextIndex = theIndex; theNextIndex < theIndex + theValue.ColSpan; theNextIndex++) {
                            TextColumn<TColumn> theNextTextColumn = theResult[theNextIndex];
                            theNextTextColumn.Width++;

                            theTotalWidth++;
                            if (theTotalWidth == theValue.CellValue.Length) {
                                break;
                            }
                        }
                    }
                }

                theColumnCount -= theValue.ColSpan - 1;
                theColumnIndex += theValue.ColSpan;
            }
        }

        return theResult;
    }

    #endregion
}
