#if DEBUG

using System;
using System.ComponentModel;
using NUnit.Framework;

namespace Epos.Utilities;

[TestFixture]
public class DebugThrowTest
{
    [Test]
    public void IfNull() {
        object? param = null;
        Assert.Throws<ArgumentNullException>(() => DebugThrow.IfNull(param));

        DebugThrow.IfNull("not-null", "param");
    }

    [Test]
    public void IfInvalidEnum() {
        var param = (DateTimeKind) 999;

        Assert.Throws<InvalidEnumArgumentException>(() => DebugThrow.IfInvalidEnum(param));

        DebugThrow.IfInvalidEnum(DateTimeKind.Utc, "param");
    }

    [Test]
    public void If() {
        Assert.Throws<ArgumentException>(() => DebugThrow.If(true, "Wrong param value.", "param"));

        DebugThrow.If(false, "Wrong param value.", "param");
    }
}

#endif
