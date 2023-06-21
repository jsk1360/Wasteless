using Refit;

namespace WastelessFunction.Lib.Biovaaka;

public interface IBiovaakaAuthClient
{
    [Post("/oauth2/v2.0/token")]
    Task<TokenRequestResult> GetToken([Body(BodySerializationMethod.UrlEncoded)] TokenRequestParameters measurement);
}