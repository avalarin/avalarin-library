namespace Avalarin.Web.Security {
    public interface ISessionProvider {
        void SaveSession(Session session);
        Session GetSession();
        void ClearSession();
    }
}