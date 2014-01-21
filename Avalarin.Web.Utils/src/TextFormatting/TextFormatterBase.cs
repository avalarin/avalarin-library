using System.Collections.Specialized;

namespace Avalarin.Web.TextFormatting {
    public interface ITextFormatter {
        string Format(string source);
        string Format(string source, NameValueCollection values);
    }
}