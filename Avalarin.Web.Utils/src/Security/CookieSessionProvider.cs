using System;
using System.Web;

namespace Avalarin.Web.Security {
    public class CookieSessionProvider : ISessionProvider {
        public const string DefaultCookieName = "__auth";

        public string CookieName { get; set; }
        public HttpContext HttpContext { get; set; }

        public CookieSessionProvider()
            : this(DefaultCookieName) {
        }

        public CookieSessionProvider(string cookieName) {
            if (cookieName == null) throw new ArgumentNullException("cookieName");
            CookieName = cookieName;
            HttpContext = HttpContext.Current;
        }
        
        public void SaveSession(Session session) {
            var cookie = new HttpCookie(CookieName, session.Key.ToString()) {
                Expires = session.Expires,
                HttpOnly = true
            };
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public Session GetSession() {
            var cookie = HttpContext.Current.Request.Cookies[CookieName];
            if (cookie == null) {
                return null;
            }
            Guid sessionKey;
            if (!Guid.TryParse(cookie.Value, out sessionKey)) {
                return null;
            }
            return new Session(sessionKey, cookie.Expires, null);
        }

        public void ClearSession() {
            HttpContext.Current.Response.Cookies.Add(new HttpCookie(CookieName, Guid.Empty.ToString()) {
                Expires = DateTime.Now, 
                HttpOnly = true
            });
        }
    }
}