using System.Data;

namespace Avalarin.Data {
    public delegate T Mapper<out T>(IDataReader reader);
}
