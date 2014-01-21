using System;
using System.Collections.Specialized;
using Avalarin.Utils;

namespace Avalarin.Web.TextFormatting {
    public static class TextFormatterExtensions {

        public static string Format(this ITextFormatter formatter, string source, object values) {
            if (formatter == null) throw new ArgumentNullException("formatter");
            if (source == null) throw new ArgumentNullException("source");
            if (values == null) {
                return formatter.Format(source); 
            } 
            var dict = values.ToDictionary();
            var nvc = new NameValueCollection();
            foreach (var kvp in dict) {
                nvc.Add(kvp.Key, kvp.Value.ToString());
            }
            return formatter.Format(source, nvc);
        }

    }
}