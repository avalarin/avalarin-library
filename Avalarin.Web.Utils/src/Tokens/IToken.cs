using System;

namespace Avalarin.Web.Tokens {
    public interface IToken {
        Type FactoryType { get; }

        void Save(ISerializationContext context);
    }
}