using System;

namespace Avalarin.Web.Tokens {
    public interface ITokenStore {
        string SaveToken(DateTime expires, IToken token);
        IToken GetToken(string key);
        void CloseToken(string key);

        void UpdateToken(string key, DateTime? expires, IToken token);
    }
}