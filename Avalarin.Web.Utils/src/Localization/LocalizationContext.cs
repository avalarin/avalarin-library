using System;
using System.Dynamic;
using System.Globalization;

namespace Avalarin.Web.Localization {
    public class LocalizationContext : DynamicObject {
        private readonly CultureInfo _culture;
        private readonly String _path;
        private readonly ILocalizationRepository _localizationRepository;

        public LocalizationContext(ILocalizationRepository localizationRepository, CultureInfo culture, string path) {
            if (localizationRepository == null) throw new ArgumentNullException("localizationRepository");
            if (culture == null) throw new ArgumentNullException("culture");
            if (path == null) throw new ArgumentNullException("path");

            _localizationRepository = localizationRepository;
            _culture = culture;
            _path = path;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result) {
            result = Localize(binder.Name);
            return true;
        }

        public String this[String key] {
            get { return Localize(key); }
        }

        public String Localize(String key) {
            if (key == null) throw new ArgumentNullException("key");
            return _localizationRepository.Get(_culture, _path, key);
        }
    }
}