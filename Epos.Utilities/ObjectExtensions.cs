using System;

namespace Epos.Utilities
{
    public static class ObjectExtensions
    {
        public static bool HasAttribute<TAttribute>(this object obj) where TAttribute : Attribute {
            return obj.GetAttribute<TAttribute>() != null;
        }

        public static TAttribute GetAttribute<TAttribute>(this object obj) where TAttribute : Attribute {
            if (obj == null) {
                throw new ArgumentNullException(nameof(obj));
            }

            return (TAttribute) Attribute.GetCustomAttribute(obj.GetType(), typeof(TAttribute));
        }
    }
}