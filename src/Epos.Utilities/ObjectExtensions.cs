using System;

namespace Epos.Utilities
{
    /// <summary>Collection of extension methods for every
    /// <see cref="System.Object" />.</summary>
    public static class ObjectExtensions
    {
        /// <summary> Determines whether the specified <see cref="System.Object"/> has
        /// the attribute <typeparamref name="TAttribute"/>.</summary>
        /// <typeparam name="TAttribute">Attribute type</typeparam>
        /// <param name="obj">Object</param>
        /// <returns><b>true</b>, if the object has the specified attribute; otherwise <b>false</b></returns>
        public static bool HasAttribute<TAttribute>(this object obj) where TAttribute : Attribute {
            return obj.GetAttribute<TAttribute>() != null;
        }

        /// <summary>Gets the attribute <typeparamref name="TAttribute"/> for the
        /// specified <see cref="System.Object" /> or <b>null</b>, if not found.</summary>
        /// <typeparam name="TAttribute">Attribute type</typeparam>
        /// <param name="obj">Object</param>
        /// <returns>Attribute <typeparamref name="TAttribute"/> or <b>null</b>, if not found</returns>
        public static TAttribute GetAttribute<TAttribute>(this object obj) where TAttribute : Attribute {
            if (obj == null) {
                throw new ArgumentNullException(nameof(obj));
            }

            return (TAttribute) Attribute.GetCustomAttribute(obj.GetType(), typeof(TAttribute));
        }
    }
}