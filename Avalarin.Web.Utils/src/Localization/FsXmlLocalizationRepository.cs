using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Caching;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using Avalarin.Utils;

namespace Avalarin.Web.Localization {
    public sealed class FsXmlLocalizationRepository : ILocalizationRepository, IDisposable {
        private readonly object _lockRoot = new object();
        private readonly string _rootPath;
        private readonly IDictionary<string, LocalizationDictionary> _dictionaries;
        private readonly Func<string, string> _defaultValueFormatter;
        private readonly IDictionary<string, object> _readLocks;

        public string RootPath {
            get { return _rootPath; }
        }

        public FsXmlLocalizationRepository(string rootPath, Func<string, string> defaultValueFormatter = null) {
            if (rootPath == null) throw new ArgumentNullException("rootPath");
            if (!Directory.Exists(rootPath)) {
                throw new DirectoryNotFoundException("Directory '" + rootPath + "' not found.");
            }
            _rootPath = rootPath;
            _dictionaries = new Dictionary<string, LocalizationDictionary>(StringComparer.OrdinalIgnoreCase);
            _defaultValueFormatter = defaultValueFormatter ?? (key => "{{" + key + "}}");
            _readLocks = new Dictionary<string, object>();
        }

        public string Get(CultureInfo culture, string path, string key) {
            if (culture == null) throw new ArgumentNullException("culture");
            if (path == null) throw new ArgumentNullException("path");
            if (key == null) throw new ArgumentNullException("key");
            var fileName = GetFileName(culture, path);
            var dict = GetDictionary(fileName);
            string value;
            dict.TryGetValue(key, out value);
            return value ?? MakeDefaultValue(key);
        }

        public void Set(CultureInfo culture, string path, string key, string value) {
            if (culture == null) throw new ArgumentNullException("culture");
            if (path == null) throw new ArgumentNullException("path");
            if (key == null) throw new ArgumentNullException("key");
            var newValue = value ?? MakeDefaultValue(key);
            var fileName = GetFileName(culture, path);
            var dict = GetDictionary(fileName);
            dict[key] = newValue;
            dict.Save();
        }

        public void Set(CultureInfo culture, string path, IDictionary<string, string> values) {
            if (culture == null) throw new ArgumentNullException("culture");
            if (path == null) throw new ArgumentNullException("path");
            if (values == null) throw new ArgumentNullException("values");
            var fileName = GetFileName(culture, path);
            var dict = GetDictionary(fileName);
            foreach (var kvp in values) {
                dict[kvp.Key] = kvp.Value;
            }
            dict.Save();
        }

        public void Dispose() {
            foreach (var dict in _dictionaries.Values) {
                dict.Dispose();
            }
        }

        private LocalizationDictionary GetDictionary(string fileName) {
            LocalizationDictionary dictionary;
            if (_dictionaries.TryGetValue(fileName, out dictionary) && !dictionary.HasChanged) {
                return dictionary;
            }
            var lockObject = GetReadLockObject(fileName);
            lock (lockObject) {
                if (_dictionaries.TryGetValue(fileName, out dictionary) && !dictionary.HasChanged) {
                    return dictionary;
                }
                var fullPath = Path.Combine(_rootPath, fileName);
                if (dictionary != null) {
                    dictionary.Reload();
                    return dictionary;
                } 
                if (!File.Exists(fullPath)) {
                    return LocalizationDictionary.CreateEmpty(fullPath);
                }
                dictionary = LocalizationDictionary.Load(fullPath);
                _dictionaries[fileName] = dictionary;
                return dictionary;
            }
        }

        private object GetReadLockObject(string fileName) {
            object lockObject;
            if (_readLocks.TryGetValue(fileName, out lockObject)) return lockObject;
            lock (_readLocks) {
                if (_readLocks.TryGetValue(fileName, out lockObject)) return lockObject;
                lockObject = new object();
                _readLocks[fileName] = lockObject;
                return lockObject;
            }
        }

        private string MakeDefaultValue(string key) {
            return _defaultValueFormatter(key);
        }

        private string GetFileName(CultureInfo culture, string path) {
            var p = path.TrimStart(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            p = p.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            return Path.Combine(culture.TwoLetterISOLanguageName, p + ".xml");
        }

        private sealed class LocalizationDictionary : IDictionary<string, string>, IDisposable {
            
            public static LocalizationDictionary Load(string fileName) {
                if (fileName == null) throw new ArgumentNullException("fileName");
                if (!File.Exists(fileName)) throw new FileNotFoundException("File '" + fileName + "' not found.", fileName);
                var dict = new LocalizationDictionary(fileName);
                dict.Reload();
                return dict;
            }

            public static LocalizationDictionary CreateEmpty(string fileName) {
                if (fileName == null) throw new ArgumentNullException("fileName");
                return new LocalizationDictionary(fileName);
            }

            private readonly IDictionary<string, string> _dictionary;
            private readonly string _fileName;
            private FileChangeMonitor _changeMonitor;

            private LocalizationDictionary(string fileName) {
                if (fileName == null) throw new ArgumentNullException("fileName");
                _dictionary = new Dictionary<string, string>();
                _fileName = fileName;
            }

            public bool HasChanged {
                get { return _changeMonitor != null && _changeMonitor.HasChanged; }
            }

            #region IDictionary<string, string>
            public IEnumerator<KeyValuePair<string, string>> GetEnumerator() {
                return _dictionary.GetEnumerator();
            }

            public void Add(KeyValuePair<string, string> item) {
                _dictionary.Add(item);
            }

            public void Clear() {
                _dictionary.Clear();
            }

            public bool Contains(KeyValuePair<string, string> item) {
                return _dictionary.Contains(item);
            }

            public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex) {
                _dictionary.CopyTo(array, arrayIndex);
            }

            public bool Remove(KeyValuePair<string, string> item) {
                return _dictionary.Remove(item);
            }

            public int Count {
                get { return _dictionary.Count; }
            }

            public bool IsReadOnly {
                get { return _dictionary.IsReadOnly; }
            }

            public bool ContainsKey(string key) {
                return _dictionary.ContainsKey(key);
            }

            public void Add(string key, string value) {
                _dictionary.Add(key, value);
            }

            public bool Remove(string key) {
                return _dictionary.Remove(key);
            }

            public bool TryGetValue(string key, out string value) {
                return _dictionary.TryGetValue(key, out value);
            }

            public string this[string key] {
                get { return _dictionary[key]; }
                set { _dictionary[key] = value; }
            }

            public ICollection<string> Keys {
                get { return _dictionary.Keys; }
            }

            public ICollection<string> Values {
                get { return _dictionary.Values; }
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return GetEnumerator();
            }
            #endregion

            public void Reload() {
                var schema = XmlUtility.GetXmlSchemaFromResource("Avalarin.Web.res.LocalizationDictionaryFile.xsd");
                var settings = new XmlReaderSettings() {
                    ConformanceLevel = ConformanceLevel.Auto,
                    ValidationType = ValidationType.Schema
                };
                settings.Schemas.Add(schema);
                using (var xmlReader = XmlReader.Create(_fileName, settings)) {
                    var xml = XDocument.Load(xmlReader);
                    foreach (var item in xml.Elements("items").Elements("item")) {
                        _dictionary[item.Attribute("key").Value] = item.Value;
                    }
                }
                UpdateMonitor();
            }

            public void Save() {
                var directoryName = Path.GetDirectoryName(_fileName);
                if (directoryName == null) {
                    throw new InvalidOperationException("Cannot get directory name from path '" + _fileName + "'.");
                }
                if (!Directory.Exists(directoryName)) {
                    Directory.CreateDirectory(directoryName);
                }
                using (var stream = new FileStream(_fileName, FileMode.OpenOrCreate))
                using (var xmlWriter = XmlWriter.Create(stream)) {
                    xmlWriter.WriteStartElement("items");
                    foreach (var kvp in _dictionary) {
                        xmlWriter.WriteStartElement("item");
                        xmlWriter.WriteAttributeString("key", kvp.Key);
                        xmlWriter.WriteString(kvp.Value);
                        xmlWriter.WriteEndElement();
                    }
                    xmlWriter.WriteEndElement();
                }
                UpdateMonitor();
            }

            public void UpdateMonitor() {
                DisposeMonitor();
                Interlocked.Exchange(ref _changeMonitor, new HostFileChangeMonitor(new[] { _fileName }));
            }

            public void Dispose() {
                DisposeMonitor();
            }

            private void DisposeMonitor() {
                if (_changeMonitor != null) {
                    _changeMonitor.Dispose();
                    _changeMonitor = null;
                }
            }
        }
    }
}