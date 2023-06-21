using System.Net.Http.Headers;
using Microsoft.Extensions.Options;

namespace WastelessFunction.Lib.Biovaaka;

public class BiovaakaAuthHeaderHandler : DelegatingHandler
{
    private readonly IBiovaakaAuthClient _authTokenStore;
    private readonly BiovaakaAuthOptions _tokenRequestParameters;

    public BiovaakaAuthHeaderHandler(IBiovaakaAuthClient authTokenStore,
        IOptions<BiovaakaAuthOptions> tokenReqParameterOptions)
    {
        _authTokenStore = authTokenStore ?? throw new ArgumentNullException(nameof(authTokenStore));
        _tokenRequestParameters = tokenReqParameterOptions.Value;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var tokenRequest = new TokenRequestParameters
        {
            Id = _tokenRequestParameters.Id,
            Scope = _tokenRequestParameters.Scope,
            Secret = _tokenRequestParameters.Secret
        };
        var token = await _authTokenStore.GetToken(tokenRequest);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}