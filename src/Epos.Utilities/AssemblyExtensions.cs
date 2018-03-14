using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Epos.Utilities
{
    /// <summary>Collection of extension methods for the <see cref="System.Reflection.Assembly" />
    /// type.</summary>
    public static class AssemblyExtensions
    {
        /// <summary>Determines whether the specified <see cref="System.Reflection.Assembly" />
        /// has the attribute <typeparamref name="TAttribute" />.</summary>
        /// <typeparam name="TAttribute">Attribute type</typeparam>
        /// <param name="assembly">Assembly</param>
        /// <returns><b>true</b>, if the assembly has the specified attribute; otherwise <b>false</b></returns>
        public static bool HasAttribute<TAttribute>(this Assembly assembly) where TAttribute : Attribute {
            return assembly.GetAttribute<TAttribute>() != null;
        }

        /// <summary>Gets the attribute <typeparamref name="TAttribute"/> for the
        /// specified <see cref="System.Reflection.Assembly" /> or <b>null</b>, if not found.</summary>
        /// <typeparam name="TAttribute">Attribute type</typeparam>
        /// <param name="assembly">Assembly</param>
        /// <returns>Attribute <typeparamref name="TAttribute"/> or <b>null</b>, if not found</returns>
        public static TAttribute GetAttribute<TAttribute>(this Assembly assembly) where TAttribute : Attribute {
            if (assembly == null) {
                throw new ArgumentNullException(nameof(assembly));
            }

            return (TAttribute) Attribute.GetCustomAttribute(assembly, typeof(TAttribute));
        }

        /// <summary>Gets the attributes <typeparamref name="TAttribute"/> for the
        /// specified <see cref="System.Reflection.Assembly" />.</summary>
        /// <typeparam name="TAttribute">Attribute type</typeparam>
        /// <param name="assembly">Assembly</param>
        /// <returns>Attributes <typeparamref name="TAttribute"/></returns>
        public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this Assembly assembly)
            where TAttribute : Attribute {
            if (assembly == null) {
                throw new ArgumentNullException(nameof(assembly));
            }

            return Attribute.GetCustomAttributes(assembly, typeof(TAttribute)).Cast<TAttribute>();
        }
    }
}
