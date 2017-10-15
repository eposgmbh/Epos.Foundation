using System;
using System.ComponentModel;
using NUnit.Framework;

namespace Epos.Utilities
{
#if DEBUG
    [TestFixture]
    public class DebugThrowTest
    {
        [Test]
        public void IfNull() {
            Assert.Throws<ArgumentNullException>(() => DebugThrow.IfNull((object) null, "param"));

            DebugThrow.IfNull("not-null", "param");
        }

        [Test]
        public void IfInvalidEnum() {
            DateTimeKind theDateTimeKind = (DateTimeKind) 999;

            Assert.Throws<InvalidEnumArgumentException>(() => DebugThrow.IfInvalidEnum(theDateTimeKind, "param"));

            DebugThrow.IfInvalidEnum(DateTimeKind.Utc, "param");
        }

        [Test]
        public void If() {
            Assert.Throws<ArgumentException>(() => DebugThrow.If(true, "Wrong param value.", "param"));

            DebugThrow.If(false, "Wrong param value.", "param");
        }
    }
#endif
}
