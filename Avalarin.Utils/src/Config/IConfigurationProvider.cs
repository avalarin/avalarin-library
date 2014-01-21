namespace Avalarin.Config {
    public interface IConfigurationProvider {
        void SetValue<T>(string key, T value);
        T GetValue<T>(string key, T defaultValue = default(T));
        void Save();
    }
}