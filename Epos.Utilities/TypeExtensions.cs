using System;
using System.Linq;
using System.Reflection;

namespace Epos.Utilities
{
    public static class TypeExtensions
    {
        public static MethodInfo GetGenericMethod(
            this Type type, string name, BindingFlags bindingAttr, Type[] typeArguments, Type[] paramTypes
        ) {
            if (type == null) {
                throw new ArgumentNullException(nameof(type));
            }
            if (name == null) {
                throw new ArgumentNullException(nameof(name));
            }
            if (typeArguments == null) {
                throw new ArgumentNullException(nameof(typeArguments));
            }
            if (paramTypes == null) {
                throw new ArgumentNullException(nameof(paramTypes));
            }

            return (
                from theMethodInfo in type.GetMethods(bindingAttr)
                where theMethodInfo.Name == name
                let theParameterInfos = theMethodInfo.GetParameters()
                where theParameterInfos.Length == paramTypes.Length
                let theGenericMethodInfo = theMethodInfo.MakeGenericMethod(typeArguments)
                where theGenericMethodInfo.GetParameters().Select(p => p.ParameterType).SequenceEqual(paramTypes)
                select theGenericMethodInfo
            ).SingleOrDefault();
        }

        public static bool HasAttribute<TAttribute>(this Type type) where TAttribute : Attribute {
            return type.GetAttribute<TAttribute>() != null;
        }

        public static TAttribute GetAttribute<TAttribute>(this Type type) where TAttribute : Attribute {
            if (type == null) {
                throw new ArgumentNullException(nameof(type));
            }

            return (TAttribute) Attribute.GetCustomAttribute(type, typeof(TAttribute));
        }
    }
}
