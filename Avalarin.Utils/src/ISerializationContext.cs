using System.Collections.Generic;

namespace Avalarin {
    public interface ISerializationContext {
        void SetValue(string key, object value);
        object GetValue(string key);
        bool Contains(string key);
        IEnumerable<string> GetKeys();
    }
}