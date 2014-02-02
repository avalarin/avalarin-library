using System;
using System.Collections.Generic;
using System.Data;

namespace Avalarin.Data {
    public static class ReaderUtils {

        public static IEnumerable<T> ReadAll<T>(this IDataReader reader, Mapper<T> mapper) {
            return reader.ReadAll((r, i1) => mapper(r));
        }

        public static IEnumerable<T> ReadAll<T>(this IDataReader reader, MapperWithIndex<T> mapper) {
            if (reader == null) {
                throw new ArgumentNullException("reader");
            }
            if (mapper == null) {
                throw new ArgumentNullException("mapper");
            }
            var set = new List<T>();
            var i = 0;
            while (reader.Read()) {
                set.Add(mapper(reader, i));
                i++;
            }
            return set.AsReadOnly();
        }

        public static T ReadFirstOrDefault<T>(this IDataReader reader, Mapper<T> mapper) {
            if (reader == null) {
                throw new ArgumentNullException("reader");
            }
            if (mapper == null) {
                throw new ArgumentNullException("mapper");
            }
            if (!reader.Read()) {
                return default(T);
            }
            return mapper(reader);
        }

        public static T Value<T>(this IDataReader reader, string name) {
            if (reader == null) throw new ArgumentNullException("reader");
            if (name == null) throw new ArgumentNullException("name");
            var ordinal = GetOrdinal<T>(reader, name);
            try {
                return (T)reader.GetValue(ordinal);
            }
            catch (InvalidCastException invalidCastException) {
                var readerType = reader.GetFieldType(ordinal);
                var readerValue = reader.IsDBNull(ordinal) ? "<NULL>" : reader[ordinal];
                throw new InvalidCastException(string.Format("Cannot cast {0}({1}) to {2}. Field name: {3}.", readerType, readerValue, typeof(T), name), invalidCastException);
            }
            
        }

        public static T ValueOrDefault<T>(this IDataReader reader, string name, T defaultValue = default(T)) {
            if (reader == null) throw new ArgumentNullException("reader");
            if (name == null) throw new ArgumentNullException("name");
            var ordinal = GetOrdinal<T>(reader, name);
            if (reader.IsDBNull(ordinal)) return defaultValue;
            return Value<T>(reader, name);
        }

        private static int GetOrdinal<T>(IDataReader reader, string name) {
            try {
                return reader.GetOrdinal(name);
            }
            catch (IndexOutOfRangeException e) {
                throw new IndexOutOfRangeException("Field '" + name + "' not found.", e);
            }
        }
    }
}