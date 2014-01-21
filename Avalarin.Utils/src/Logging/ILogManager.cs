namespace Avalarin.Logging {
    public interface ILogManager {
        ILogger GetLogger(string name);
    }
}