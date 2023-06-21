using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace WastelessFunction.Lib.Predictions;

public class AuthHeaderHandler : DelegatingHandler
{
    private readonly string _code;

    public AuthHeaderHandler(IOptions<PredictionOptions> options)
    {
        _code = options.Value.Code;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (request.RequestUri is not null)
        {
            var url = QueryHelpers.AddQueryString(request.RequestUri.ToString(), "code", _code);
            request.RequestUri = new Uri(url);
        }

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}