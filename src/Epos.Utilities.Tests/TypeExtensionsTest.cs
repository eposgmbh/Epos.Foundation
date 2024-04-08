using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using NUnit.Framework;

namespace Epos.Utilities;

[TestFixture]
public class TypeExtensionsTest
{
    [Test]
    public void GetGenericMethod() {
        Assert.Throws<ArgumentNullException>(
            () => TypeExtensions.GetGenericMethod(null!, "0", BindingFlags.Default, [], [])
        );
        Assert.Throws<ArgumentNullException>(
            () => typeof(int).GetGenericMethod(null!, BindingFlags.Default, [], [])
        );
        Assert.Throws<ArgumentNullException>(
            () => typeof(int).GetGenericMethod("0", BindingFlags.Default, null!, [])
        );
        Assert.Throws<ArgumentNullException>(
            () => typeof(int).GetGenericMethod("0", BindingFlags.Default, [], null!)
        );

        MethodInfo? theMethodInfo = typeof(Enumerable).GetGenericMethod(
            "Empty", BindingFlags.Static | BindingFlags.Public,
            [typeof(string)], []
        );
        Assert.That(theMethodInfo, Is.Not.Null);

        theMethodInfo = typeof(Enumerable).GetGenericMethod(
            "Single", BindingFlags.Static | BindingFlags.Public,
            [typeof(int)], [typeof(IEnumerable<int>)]
        );
        Assert.That(theMethodInfo, Is.Not.Null);
    }

    [Test]
    public void HasAttribute() {
        Assert.Throws<ArgumentNullException>(() => TypeExtensions.HasAttribute<AttributeUsageAttribute>(null!));

        Assert.That(typeof(AssemblyCompanyAttribute).HasAttribute<AttributeUsageAttribute>(), Is.True);
        Assert.That(typeof(int).HasAttribute<AttributeUsageAttribute>(), Is.False);
    }

    [Test]
    public void GetAttribute()
    {
        Assert.Throws<ArgumentNullException>(() => TypeExtensions.GetAttribute<AttributeUsageAttribute>(null!));

        AttributeUsageAttribute? attribute = typeof(AssemblyCompanyAttribute).GetAttribute<AttributeUsageAttribute>();
        Assert.That(attribute, Is.Not.Null);
        Assert.That(attribute!.ValidOn, Is.EqualTo(AttributeTargets.Assembly));
        Assert.That(typeof(int).GetAttribute<AttributeUsageAttribute>(), Is.Null);
    }
}
