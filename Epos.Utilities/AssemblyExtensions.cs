using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Epos.Utilities
{
    public static class AssemblyExtensions
    {
        public static bool HasAttribute<TAttribute>(this Assembly assembly) where TAttribute : Attribute {
            if (assembly == null) {
                throw new ArgumentNullException(nameof(assembly));
            }

            return assembly.GetCustomAttribute<TAttribute>() != null;
        }

        public static bool HasAttribute<TAttribute>(this Type type) where TAttribute : Attribute {
            if (type == null) {
                throw new ArgumentNullException(nameof(type));
            }

            return Attribute.GetCustomAttribute(type, typeof(TAttribute)) != null;
        }

        public static TAttribute GetAttribute<TAttribute>(this Type type) where TAttribute : Attribute {
            if (type == null) {
                throw new ArgumentNullException(nameof(type));
            }

            return (TAttribute) Attribute.GetCustomAttribute(type, typeof(TAttribute));
        }

        public static TAttribute GetAttribute<TAttribute>(this object obj) where TAttribute : Attribute {
            if (obj == null) {
                throw new ArgumentNullException(nameof(obj));
            }

            return (TAttribute) Attribute.GetCustomAttribute(obj.GetType(), typeof(TAttribute));
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