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
            Assert.That(typeof(DateTime).Dump(), Is.EqualTo("System.DateTime"));
            Assert.That(typeof(Cache<,>).Dump(), Is.EqualTo("Epos.Utilities.Cache<TKey, TValue>"));
            Assert.That(typeof(Cache<int, string>).Dump(), Is.EqualTo("Epos.Utilities.Cache<int, string>"));
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

        #region Hilfsmember

        private static IEnumerable GetEnumerable1() {
            return new[] { 1, 2, 3 };
        }

        private static IEnumerable GetEnumerable2() {
            return new[] { new DictionaryEntry(2, "Two"), new DictionaryEntry(3, "Three") };
        }

        private sealed class ExampleClass
        {
            #pragma warning disable 169
            private int myOne = 1;
            
            // ReSharper disable InconsistentNaming
            private string _two = "Two";
            // ReSharper disable once UnusedMember.Local
            private DateTime date = new DateTime(2099, 9, 9);
            // ReSharper restore InconsistentNaming
            
            #pragma warning restore 169
        }

        #endregion
    }
}