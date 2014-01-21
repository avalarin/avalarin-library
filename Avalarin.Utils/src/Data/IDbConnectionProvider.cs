using System.Data;

namespace Avalarin.Data {
    public interface IDbConnectionProvider {
        IDbConnection GetConnection();
    }
}