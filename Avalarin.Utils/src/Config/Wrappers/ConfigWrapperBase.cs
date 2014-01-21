using System.Collections.Generic;

namespace Avalarin.Config.Wrappers {
    public abstract class ConfigWrapperBase {
        private readonly IConfigurationProvider _config;
        private readonly IDictionary<string, object> _values;

        public ConfigWrapperBase(IConfigurationProvider config) {
            _config = config;
            _values = new Dictionary<string, object>();
        }

        private IConfigurationProvider Config {
            get { return _config; }
        }

        private IDictionary<string, object> Values {
            get { return _values; }
        }

        protected void SetValue<T>(string key, T value) {
            _values[key] = value;
        }

        protected T GetValue<T>(string key, T defaultValue = default(T)) {
            object value;
            if (Values.TryGetValue(key, out value)) {
                return (T) value;
            }
            return Config.GetValue<T>(key, defaultValue);
        }

        public void Save() {
            foreach (var item in Values) {
                Config.SetValue(item.Key, item.Value);
            }
            Config.Save();
        }

        public void Reset() {
            Values.Clear();
        }

    }
}