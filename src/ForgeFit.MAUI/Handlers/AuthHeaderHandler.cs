using System.Net.Http.Headers;
using ForgeFit.MAUI.Constants;

namespace ForgeFit.MAUI.Handlers;

public class AuthHeaderHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await SecureStorage.GetAsync(AuthConstants.AccessToken);

        if (!string.IsNullOrEmpty(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }
}
