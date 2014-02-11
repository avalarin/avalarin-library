using System;
using System.Data;

namespace Avalarin.Data {
    public static class SqlExtensions {
        public static DbCommandWrapper Sp(this IDbConnection connection, string text) {
            if (connection == null) throw new ArgumentNullException("connection");
            if (text == null) throw new ArgumentNullException("text");
            return DbCommandWrapper.CreateSp(connection, text);
        }

        public static DbCommandWrapper Text(this IDbConnection connection, string text) {
            if (connection == null) throw new ArgumentNullException("connection");
            if (text == null) throw new ArgumentNullException("text");
            return DbCommandWrapper.CreateText(connection, text);
        }
    }
}