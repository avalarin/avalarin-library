namespace Avalarin.Web.Tokens {
    public interface ITokenFactory {
        IToken CreateToken(ISerializationContext context);
    }
}