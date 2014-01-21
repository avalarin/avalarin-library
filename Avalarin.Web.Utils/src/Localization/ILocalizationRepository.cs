using System.Collections.Generic;
using System.Globalization;

namespace Avalarin.Web.Localization {
    public interface ILocalizationRepository {
        string Get(CultureInfo culture, string path, string key);
        void Set(CultureInfo culture, string path, string key, string value);
        void Set(CultureInfo culture, string path, IDictionary<string, string> values);
    }
}