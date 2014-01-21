using System.Collections.Specialized;
using System.Configuration;
using Avalarin.Config;
using Avalarin.Utils;

namespace Avalarin.Web.Config {
    public class AppConfigurationProvider : IConfigurationProvider {

        private readonly NameValueCollection _values;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public AppConfigurationProvider() {
            _values = new NameValueCollection();
        }

        public void SetValue<T>(string key, T value) {            
            _values[key] = SimpleSerializer.Serialize(value);
        }

        public T GetValue<T>(string key, T defaultValue = default(T)) {
            var stringValue = _values[key] ?? System.Configuration.ConfigurationManager.AppSettings[key];
            return SimpleSerializer.Deserialize(stringValue, defaultValue);
        }

        public void Save() {
            var config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            foreach (var key in _values.AllKeys) {
                var s = config.AppSettings.Settings[key];
                if (s != null) {
                    s.Value = _values[key];
                }
                else {
                    config.AppSettings.Settings.Add(key, _values[key]);
                }
                _values.Remove(key);
            }
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

    }
}