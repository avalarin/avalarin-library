using System;
using System.Collections.Generic;
using Avalarin.Config;
using Avalarin.Config.Wrappers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Utils {
    [TestClass]
    public sealed class ConfigWrappersTest {

        [TestMethod]
        public void SimpleWrapperTest() {
            var config = new FakeConfigurationProvider();
            const string oldName = "Razor";
            const string newName = "Logitech";
            config.SetValue("Name", oldName);
            var wrapper = new SimpleWrapper(config);
            Assert.AreEqual(oldName, config.GetValue<string>("Name"));
            Assert.AreEqual(config.GetValue<string>("Name"), wrapper.Name);
            wrapper.Name = newName;
            Assert.AreEqual(config.GetValue<string>("Name"), oldName);
            Assert.AreEqual(wrapper.Name, newName);
            wrapper.Reset();
            Assert.AreEqual(config.GetValue<string>("Name"), oldName);
            Assert.AreEqual(wrapper.Name, oldName);
            wrapper.Name = newName;
            wrapper.Save();
            Assert.AreEqual(wrapper.Name, config.GetValue<string>("Name"));
        }

        private sealed class SimpleWrapper : ConfigWrapperBase {
            public SimpleWrapper(IConfigurationProvider config) : base(config) {
            }

            public string Name {
                get { return GetValue<string>("Name"); }
                set { SetValue("Name", value); }
            }

        }

        private sealed class FakeConfigurationProvider : IConfigurationProvider {
            private readonly IDictionary<string, object> _values;

            public FakeConfigurationProvider() {
                _values = new Dictionary<string, object>();
            }

            public void SetValue<T>(string key, T value) {
                if (key == null) throw new ArgumentNullException("key");
                _values[key] = value;
            }

            public T GetValue<T>(string key, T defaultValue = default(T)) {
                if (key == null) throw new ArgumentNullException("key");
                object value;
                if (!_values.TryGetValue(key, out value)) {
                    return defaultValue;
                }
                return (T) value;
            }

            public void Save() { }
        }

    }
}