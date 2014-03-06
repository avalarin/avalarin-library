using System.Globalization;

namespace Avalarin.Web.Localization {
    public static class LocalizationRepositoryExtensions {

        public static LocalizationContext CreateContext(this ILocalizationRepository repository, CultureInfo cultureInfo, string path) {
            return new LocalizationContext(repository, cultureInfo, path);
        }

    }
}