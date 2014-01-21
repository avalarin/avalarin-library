using System;
using System.Linq;
using System.Reflection;

namespace Avalarin.Utils {
    public static class TypeExtensions {

        public static MethodInfo GetGenericMethod(this Type type, string name, BindingFlags flags, Func<Type, Type[]> typesProvider) {
            return GetGenericMethod(type, name, flags, 1, types => typesProvider(types[0]));
        }

        public static MethodInfo GetGenericMethod(this Type type, string name, BindingFlags flags, Func<Type, Type, Type[]> typesProvider) {
            return GetGenericMethod(type, name, flags, 2, types => typesProvider(types[0], types[1]));
        }

        public static MethodInfo GetGenericMethod(this Type type, string name, BindingFlags flags, Func<Type, Type, Type, Type[]> typesProvider) {
            return GetGenericMethod(type, name, flags, 3, types => typesProvider(types[0], types[1], types[2]));
        }

        public static MethodInfo GetGenericMethod(this Type type, string name, BindingFlags flags, int genericArgsCount, Func<Type[], Type[]> typesProvider) {
            var nameComparer = ((flags & BindingFlags.IgnoreCase) != 0) ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
            var methods = type.GetMethods(flags).Where(m => nameComparer.Equals(name, m.Name));
            MethodInfo matched = null;
            foreach (var method in methods) {
                var genericArgs = method.GetGenericArguments();
                if (genericArgs.Length != genericArgsCount) continue;
                var types = typesProvider(genericArgs);
                if (!method.GetParameters().Select(p => p.ParameterType).SequenceEqual(types)) continue;
                if (matched != null) {
                    throw new AmbiguousMatchException("More than one matching method found.");
                }
                matched = method;
            }
            return matched;
        }

        public static bool IsPrimitiveOrStringOrDateTime(this Type type) {
            return type == typeof(string) || type.IsPrimitive || type == typeof(DateTime);
        }

        public static bool IsNumericType(this Type type) {
            if (type == null) {
                return false;
            }

            switch (Type.GetTypeCode(type)) {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                case TypeCode.Object:
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)) {
                        return IsNumericType(Nullable.GetUnderlyingType(type));
                    }
                    return false;
            }
            return false;
        }

        public static bool IsNullableType(this Type theType) {
            return (theType.IsGenericType &&
            theType.GetGenericTypeDefinition() == typeof(Nullable<>));
        }
    }
}