using System;

namespace Avalarin.Web.Security {
    public class Session {
        public Guid Key { get; private set; }
        public DateTime Expires { get; private set; }
        public bool? IsPersistent { get; private set; }

        public Session(Guid key, DateTime expires, bool? isPersistent) {
            Key = key;
            Expires = expires;
            IsPersistent = isPersistent;
        }
    }
}