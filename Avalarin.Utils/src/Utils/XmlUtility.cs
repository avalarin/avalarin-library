using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Xml.Schema;

namespace Avalarin.Utils {
    public static class XmlUtility {
        private static readonly Object SchemasLocker = new Object();
        private static readonly Object SerializersLocker = new Object();

        private static readonly IDictionary<Tuple<Assembly, String>, XmlSchema> Schemas =
            new Dictionary<Tuple<Assembly, String>, XmlSchema>();

        public static XmlSchema GetXmlSchemaFromResource(String resourceName) {
            return GetXmlSchemaFromResource(resourceName, Assembly.GetCallingAssembly());
        }

        public static XmlSchema GetXmlSchemaFromResource(String resourceName, Assembly assembly) {
            if (resourceName == null) throw new ArgumentNullException("resourceName");
            if (assembly == null) throw new ArgumentNullException("assembly");
            var key = new Tuple<Assembly, String>(assembly, resourceName);
            XmlSchema schema;
            if (Schemas.TryGetValue(key, out schema)) return schema;
            lock (SchemasLocker) {
                if (Schemas.TryGetValue(key, out schema)) return schema;
                using (var stream = assembly.GetManifestResourceStream(resourceName)) {
                    if (stream == null) {
                        throw new InvalidOperationException(
                            String.Format(
                                CultureInfo.CurrentCulture,
                                "Schema '{0}' not found in the assembly '{1}'.",
                                resourceName,
                                assembly.FullName));
                    }
                    Schemas.Add(key, (schema = XmlSchema.Read(stream, null)));
                }
            }
            return schema;
        }
    }
}