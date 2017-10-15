using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Epos.Utilities
{
    public static class AssemblyExtensions
    {
        public static bool HasAttribute<TAttribute>(this Assembly assembly) where TAttribute : Attribute {
            return assembly.GetAttribute<TAttribute>() != null;
        }

        public static TAttribute GetAttribute<TAttribute>(this Assembly assembly) where TAttribute : Attribute {
            if (assembly == null) {
                throw new ArgumentNullException(nameof(assembly));
            }

            return (TAttribute) Attribute.GetCustomAttribute(assembly, typeof(TAttribute));
        }

        public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this Assembly assembly)
            where TAttribute : Attribute {
            if (assembly == null) {
                throw new ArgumentNullException(nameof(assembly));
            }

            return Attribute.GetCustomAttributes(assembly, typeof(TAttribute)).Cast<TAttribute>();
        }
    }
}
