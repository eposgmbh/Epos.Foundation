using System;
using System.Reflection;

namespace Epos.Utilities;

/// <summary>Collection of extension methods for every
/// <see cref="object" />.</summary>
public static class ObjectExtensions
{
    /// <summary> Determines whether the specified <see cref="object"/> has
    /// the attribute <typeparamref name="TAttribute"/>.</summary>
    /// <typeparam name="TAttribute">Attribute type</typeparam>
    /// <param name="obj">Object</param>
    /// <returns><b>true</b>, if the object has the specified attribute; otherwise <b>false</b></returns>
    public static bool HasAttribute<TAttribute>(this object obj) where TAttribute : Attribute
        => obj.GetAttribute<TAttribute>() is not null;

    /// <summary>Gets the attribute <typeparamref name="TAttribute"/> for the
    /// specified <see cref="object" /> or <b>null</b>, if not found.</summary>
    /// <typeparam name="TAttribute">Attribute type</typeparam>
    /// <param name="obj">Object</param>
    /// <returns>Attribute <typeparamref name="TAttribute"/> or <b>null</b>, if not found</returns>
    public static TAttribute? GetAttribute<TAttribute>(this object obj) where TAttribute : Attribute {
        if (obj is null) {
            throw new ArgumentNullException(nameof(obj));
        }

        if (obj is MemberInfo theMemberInfo) {
            return (TAttribute?) Attribute.GetCustomAttribute(theMemberInfo, typeof(TAttribute));
        }

        return (TAttribute?) Attribute.GetCustomAttribute(obj.GetType(), typeof(TAttribute));
    }
}
