using Refit;

namespace WastelessFunction.Lib.Biovaaka;

public class TokenRequestParameters
{
    [AliasAs("client_id")] public string Id { get; set; }
    [AliasAs("scope")] public string Scope { get; set; }
    [AliasAs("client_secret")] public string Secret { get; set; }
    [AliasAs("grant_type")] public string GrantType => "client_credentials";
}