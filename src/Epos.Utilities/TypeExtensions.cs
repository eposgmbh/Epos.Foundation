using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Epos.Utilities;

/// <summary>Collection of extension methods for the
/// <see cref="System.Type" /> type.</summary>
public static class TypeExtensions
{
    /// <summary>Gets a constructed generic method with the specified <paramref name="name"/>,
    /// type arguments (<paramref name="typeArguments"/>) and parameter types (<paramref name="paramTypes"/>).
    /// </summary>
    /// <param name="type">Type</param>
    /// <param name="name">Method name</param>
    /// <param name="bindingAttr">Binding flags</param>
    /// <param name="typeArguments">Type arguments</param>
    /// <param name="paramTypes">Parameter types</param>
    /// <returns>Constructed generic <see cref="System.Reflection.MethodInfo"/></returns>
    [UnconditionalSuppressMessage("AssemblyLoadTrimming", "IL2060", Justification = "Types are available at runtime.")]
    [UnconditionalSuppressMessage("AssemblyLoadTrimming", "IL3050", Justification = "Types are available at runtime.")]
    public static MethodInfo? GetGenericMethod(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods |
         DynamicallyAccessedMemberTypes.PublicMethods)] this Type type,
         string name, BindingFlags bindingAttr, Type[] typeArguments, Type[] paramTypes
    ) {
        if (type is null) {
            throw new ArgumentNullException(nameof(type));
        }
        if (name is null) {
            throw new ArgumentNullException(nameof(name));
        }
        if (typeArguments is null) {
            throw new ArgumentNullException(nameof(typeArguments));
        }
        if (paramTypes is null) {
            throw new ArgumentNullException(nameof(paramTypes));
        }

        return (
            from theMethodInfo in type.GetMethods(bindingAttr)
            where theMethodInfo.IsGenericMethodDefinition && theMethodInfo.Name == name
            let theParameterInfos = theMethodInfo.GetParameters()
            where theParameterInfos.Length == paramTypes.Length
            let theGenericMethodInfo = theMethodInfo.MakeGenericMethod(typeArguments)
            where theGenericMethodInfo.GetParameters().Select(p => p.ParameterType).SequenceEqual(paramTypes)
            select theGenericMethodInfo
        ).SingleOrDefault();
    }

    /// <summary> Determines whether the specified <see cref="System.Type"/> has
    /// the attribute <typeparamref name="TAttribute"/>.</summary>
    /// <typeparam name="TAttribute">Attribute type</typeparam>
    /// <param name="type">Type</param>
    /// <returns><b>true</b>, if the type has the specified attribute; otherwise <b>false</b></returns>
    public static bool HasAttribute<TAttribute>(this Type type) where TAttribute : Attribute
        => type.GetAttribute<TAttribute>() is not null;

    /// <summary>Gets the attribute <typeparamref name="TAttribute"/> for the
    /// specified <see cref="System.Type" /> or <b>null</b>, if not found.</summary>
    /// <typeparam name="TAttribute">Attribute type</typeparam>
    /// <param name="type">Type</param>
    /// <returns>Attribute <typeparamref name="TAttribute"/> or <b>null</b>, if not found</returns>
    public static TAttribute? GetAttribute<TAttribute>(this Type type) where TAttribute : Attribute {
        if (type is null) {
            throw new ArgumentNullException(nameof(type));
        }

        return (TAttribute?) Attribute.GetCustomAttribute(type, typeof(TAttribute));
    }
}
