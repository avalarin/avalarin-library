namespace Avalarin.Logging.NLog {
    public sealed class NLogManager : ILogManager {
        public ILogger GetLogger(string name) {
            return new NLogLogger(global::NLog.LogManager.GetLogger(name));
        }
    }
}