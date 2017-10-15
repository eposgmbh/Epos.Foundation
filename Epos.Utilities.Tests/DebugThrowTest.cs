using System;
using System.ComponentModel;
using System.Diagnostics;
using NUnit.Framework;

namespace Epos.Utilities
{
    [TestFixture]
    public class DebugThrowTest
    {
        [Test]
        [Conditional("DEBUG")]
        public void IfNull() {
            Assert.Throws<ArgumentNullException>(() => DebugThrow.IfNull((object) null, "param"));

            DebugThrow.IfNull("not-null", "param");
        }

        [Test]
        [Conditional("DEBUG")]
        public void IfInvalidEnum() {
            DateTimeKind theDateTimeKind = (DateTimeKind) 999;

            Assert.Throws<InvalidEnumArgumentException>(() => DebugThrow.IfInvalidEnum(theDateTimeKind, "param"));

            DebugThrow.IfInvalidEnum(DateTimeKind.Utc, "param");
        }

        [Test]
        [Conditional("DEBUG")]
        public void If() {
            Assert.Throws<ArgumentException>(() => DebugThrow.If(true, "Wrong param value.", "param"));

            DebugThrow.If(false, "Wrong param value.", "param");
        }
    }
}
