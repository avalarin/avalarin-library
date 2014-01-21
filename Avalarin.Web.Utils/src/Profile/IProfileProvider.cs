using System;

namespace Avalarin.Web.Profile {
    public interface IProfileProvider {
        void SetValue<T>(Guid userId, string key, T value);
        T GetValue<T>(Guid userId, string key, T defaultValue = default(T));
    }
}
