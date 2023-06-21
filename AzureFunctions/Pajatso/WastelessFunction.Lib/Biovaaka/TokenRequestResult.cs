using System.Text.Json.Serialization;

namespace WastelessFunction.Lib.Biovaaka;

public class TokenRequestResult
{
    [JsonPropertyName("token_type")] public string TokenType { get; set; }
    [JsonPropertyName("expires_in")] public long ExpiresIn { get; set; }
    [JsonPropertyName("ext_expires_in")] public long ExtExpiresIn { get; set; }
    [JsonPropertyName("access_token")] public string AccessToken { get; set; }
}