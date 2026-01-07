using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ForgeFit.MAUI.Constants;
using ForgeFit.MAUI.Models.DTOs.Auth;
using ForgeFit.MAUI.Views.Auth;

namespace ForgeFit.MAUI.Handlers;

public class RefreshTokenHandler(IHttpClientFactory httpClientFactory) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode != HttpStatusCode.Unauthorized ||
            request.RequestUri!.AbsolutePath.Contains("/auth/sign-in") ||
            request.RequestUri!.AbsolutePath.Contains("/auth/sign-up"))
            return response;

        var newAccessToken = await RefreshTokenAsync(cancellationToken);

        if (!string.IsNullOrEmpty(newAccessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newAccessToken);

            return await base.SendAsync(request, cancellationToken);
        }

        SignOut();
        return response;
    }

    private async Task<string?> RefreshTokenAsync(CancellationToken cancellationToken)
    {
        try
        {
            var refreshToken = await SecureStorage.GetAsync(AuthConstants.RefreshToken);
            if (string.IsNullOrEmpty(refreshToken)) return null;

            var refreshClient = httpClientFactory.CreateClient("RefreshClient");

            var requestPayload = new RefreshTokenRequest(refreshToken);

            var response =
                await refreshClient.PostAsJsonAsync("/api/auth/refresh-token", requestPayload, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<UserSignInResponse>(cancellationToken);

                if (data != null)
                {
                    await SecureStorage.SetAsync(AuthConstants.AccessToken, data.AccessToken);
                    await SecureStorage.SetAsync(AuthConstants.RefreshToken, data.RefreshToken);
                    return data.AccessToken;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Refresh failed: {ex.Message}");
        }

        return null;
    }

    private static void SignOut()
    {
        SecureStorage.Remove(AuthConstants.AccessToken);
        SecureStorage.Remove(AuthConstants.RefreshToken);

        var loginPage = Application.Current?.Handler?.MauiContext?.Services.GetService<LoginPageView>();
        if (Application.Current?.Windows.Count > 0) Application.Current.Windows[0].Page = loginPage;
    }
}
