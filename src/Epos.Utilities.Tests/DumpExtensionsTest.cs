using System;
using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;

namespace Epos.Utilities
{
    [TestFixture]
    public class DumpExtensionsTest
    {
        [Test]
        public void Null() {
            Assert.That((null as object).Dump(), Is.EqualTo("null"));
        }

        [Test]
        public void String() {
            Assert.That("String".Dump(), Is.EqualTo("String"));
        }

        [Test]
        public void DictionaryEntry() {
            Assert.That(new DictionaryEntry(1, "One").Dump(), Is.EqualTo("{ 1: One }"));
            Assert.That(
                new DictionaryEntry(2.2, new KeyValuePair<int, string>(3, "Three")).Dump(),
                Is.EqualTo("{ 2.2: { 3: Three } }")
            );
        }

        [Test]
        public void Type() {
            Assert.That(typeof(int).Dump(), Is.EqualTo("int"));
            Assert.That(typeof(string).Dump(), Is.EqualTo("string"));
            Assert.That(typeof(decimal).Dump(), Is.EqualTo("decimal"));
            Assert.That(typeof(double?).Dump(), Is.EqualTo("double?"));
            Assert.That(typeof(DateTime).Dump(), Is.EqualTo("DateTime"));
            Assert.That(typeof(DateOnly).Dump(), Is.EqualTo("DateOnly"));
            Assert.That(typeof(DefaultCache<,>).Dump(), Is.EqualTo("Epos.Utilities.DefaultCache<TKey, TValue>"));
            Assert.That(typeof(DefaultCache<int, string>).Dump(), Is.EqualTo("Epos.Utilities.DefaultCache<int, string>"));
            Assert.That(typeof(ExampleClass).Dump(), Is.EqualTo("Epos.Utilities.DumpExtensionsTest.ExampleClass"));
        }

        [Test]
        public void KeyValuePair() {
            Assert.That(new KeyValuePair<int, string>(1, "One").Dump(), Is.EqualTo("{ 1: One }"));
            Assert.That(
                new KeyValuePair<double, DictionaryEntry>(3.3, new DictionaryEntry(4, "Four")).Dump(),
                Is.EqualTo("{ 3.3: { 4: Four } }")
            );
        }

        [Test]
        public void ToStringMethodImplementation() {
            Assert.That(1.Dump(), Is.EqualTo("1")); // ToString(IFormatProvider)
            Assert.That(new Uri("uri", UriKind.Relative).Dump(), Is.EqualTo("uri"));
        }

        [Test]
        public void Enumerable() {
            Assert.That(GetEnumerable1().Dump(), Is.EqualTo("[1, 2, 3]"));
            Assert.That(GetEnumerable2().Dump(), Is.EqualTo("[{ 2: Two }, { 3: Three }]"));
        }

        [Test]
        public void Enumerator() {
            Assert.That(GetEnumerable1().GetEnumerator().Dump(), Is.EqualTo("[1, 2, 3]"));
            Assert.That(GetEnumerable2().GetEnumerator().Dump(), Is.EqualTo("[{ 2: Two }, { 3: Three }]"));
        }

        [Test]
        public void AnonymousType() {
            Assert.That(new { Integer = 1, String = "Hello" }.Dump(), Is.EqualTo("{ Integer = 1, String = Hello }"));
        }

        [Test]
        public void OtherClass() {
            Assert.That(new ExampleClass().Dump(), Is.EqualTo("{ One = 1, Two = Two, Date = 09/09/2099 00:00:00 }"));
        }

        [Test]
        public void TableObject() {
            string theActual = new MyTable().DumpTableObject(new MyTableInfoProvider());

            var theExpected = @"
            Left             |       Right      
Header1 | Header2 | Header3  | Header4 | Header5
    int | string  | string   |  double | string 
--------|---------|----------|---------|--------
      1 | Gabbel  | World    |   34.45 | Guguck 
      2 | Blofeld | Hello    |   43.89 | Moin   
      3 | Kaputt  | Together |   99.23 | Hi     
--------|---------|----------|---------|--------
Sum                          |  177.57 |        
".Substring(Environment.NewLine.Length);

            Assert.That(theActual, Is.EqualTo(theExpected));
        }

        #region Hilfsmember

        private class MyTable { }

        private class MyTableInfoProvider : TableInfoProvider<object, Row>
        {
            public override IEnumerable<object> GetColumns() {
                return new object[5];
            }

            public override ColumnInfo GetColumnInfo(object column, int columnIndex) {
                return columnIndex switch {
                    0 => new ColumnInfo {
                        Header = "Header1", AlignRight = true, DataType = "int",
                        Seperator = new ColumnSeperator { Header = "Left", ColSpan = 3 }
                    },
                    1 => new ColumnInfo { Header = "Header2", DataType = "string" },
                    2 => new ColumnInfo { Header = "Header3", DataType = "string" },
                    3 => new ColumnInfo {
                        Header = "Header4", AlignRight = true, DataType = "double",
                        Seperator = new ColumnSeperator { Header = "Right", ColSpan = 2 }
                    },
                    4 => new ColumnInfo { Header = "Header5", DataType = "string" },
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            public override IEnumerable<Row> GetRows() {
                yield return new Row(1, "Gabbel", "World", 34.45, "Guguck");
                yield return new Row(2, "Blofeld", "Hello", 43.89, "Moin");
                yield return new Row(3, "Kaputt", "Together", 99.23, "Hi");
                yield return new Row(new TableHeader("Sum", 3), (34.45 + 43.89 + 99.23), "");
            }

            public override (string CellValue, int ColSpan) GetCellValue(Row row, object column, int columnIndex) {
                var theValue = row.Values[columnIndex];

                if (theValue is TableHeader theHeader) {
                    return (theHeader.Value, theHeader.ColSpan);
                }

                return (theValue.Dump(), 1);
            }

            public override bool HasSumRow => true;
        }

        private record Row(params object[] Values);

        private record TableHeader(string Value, int ColSpan);

        private static IEnumerable GetEnumerable1() {
            return new[] { 1, 2, 3 };
        }

        private static IEnumerable GetEnumerable2() {
            return new[] { new DictionaryEntry(2, "Two"), new DictionaryEntry(3, "Three") };
        }

        private sealed class ExampleClass
        {
#pragma warning disable 169
#pragma warning disable 414
            private int myOne = 1;
            private string _two = "Two";
            private DateTime date = new DateTime(2099, 9, 9);

#pragma warning restore 169
#pragma warning disable 414
        }

        #endregion
    }
}
