using System;
using System.Globalization;

namespace Avalarin.Utils {
    public static class SimpleSerializer {

        public static string Serialize<T>(T value) {
            if ((object) value == null) {
                return string.Empty;
            }
            var asString = value as String;
            if (asString != null) {
                return asString;
            }
            var valueType = typeof(T);
            if (valueType.IsPrimitiveOrStringOrDateTime()) {
                return ((IConvertible)value).ToString(CultureInfo.InvariantCulture);
            }
            
            if (valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(Nullable<>)) {
                if (valueType.GenericTypeArguments[0].IsPrimitiveOrStringOrDateTime()) {
                    return ((IConvertible)value).ToString(CultureInfo.InvariantCulture);
                }
            }
            throw new NotSupportedException("Type '" + valueType.Name + "' not supported.");
        }

        public static T Deserialize<T>(string str, T defaultValue = default(T)) {
            if (string.IsNullOrWhiteSpace(str)) {
                return defaultValue;
            }
            var valueType = typeof (T);
            if (valueType.IsPrimitiveOrStringOrDateTime()) {
                return (T) Convert.ChangeType(str, valueType, CultureInfo.InvariantCulture);
            }
            var nullableType = typeof (Nullable<>);
            if (valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                valueType.GenericTypeArguments[0].IsPrimitiveOrStringOrDateTime()) {
                    var value = Convert.ChangeType(str, valueType.GenericTypeArguments[0], CultureInfo.InvariantCulture);
                return (T) Activator.CreateInstance(nullableType.MakeGenericType(valueType.GenericTypeArguments[0]), value);
            }
            throw new NotSupportedException("Type '" + valueType.Name + "' not supported.");
        }

    }
}