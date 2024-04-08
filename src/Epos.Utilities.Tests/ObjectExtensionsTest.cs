using System;
using System.Reflection;
using NUnit.Framework;

namespace Epos.Utilities;

[TestFixture]
public class ObjectExtensionsTest
{
    [Test]
    public void HasAttribute()
    {
        Assert.Throws<ArgumentNullException>(() => ObjectExtensions.HasAttribute<AttributeUsageAttribute>(null!));

        var theAssemblyTitleAttribute = new AssemblyTitleAttribute("Hello");
        Assert.That(theAssemblyTitleAttribute.HasAttribute<AttributeUsageAttribute>(), Is.True);
    }

    [Test]
    public void GetAttribute() {
        Assert.Throws<ArgumentNullException>(() => ObjectExtensions.GetAttribute<AttributeUsageAttribute>(null!));

        var theAssemblyTitleAttribute = new AssemblyTitleAttribute("Hello");
        AttributeUsageAttribute? theAssemblyUsageAttribute =
            theAssemblyTitleAttribute.GetAttribute<AttributeUsageAttribute>();
        Assert.That(theAssemblyUsageAttribute, Is.Not.Null);
        Assert.That(theAssemblyUsageAttribute!.ValidOn, Is.EqualTo(AttributeTargets.Assembly));
    }
}
