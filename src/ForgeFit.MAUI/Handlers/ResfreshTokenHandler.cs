using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ForgeFit.MAUI.Constants;
using ForgeFit.MAUI.Models.DTOs.Auth;
using ForgeFit.MAUI.Views.Auth;

namespace ForgeFit.MAUI.Handlers;

public class RefreshTokenHandler(IHttpClientFactory httpClientFactory) : DelegatingHandler
{
    private static readonly SemaphoreSlim Semaphore = new(1, 1);

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode != HttpStatusCode.Unauthorized ||
            request.RequestUri!.AbsolutePath.Contains("/auth/sign-in") ||
            request.RequestUri!.AbsolutePath.Contains("/auth/sign-up"))
            return response;

        await Semaphore.WaitAsync(cancellationToken);
        try
        {
            var currentAccessToken = await SecureStorage.GetAsync(AuthConstants.AccessToken);

            if (IsTokenDifferent(request, currentAccessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", currentAccessToken);
                response.Dispose();
                return await base.SendAsync(request, cancellationToken);
            }

            var newAccessToken = await RefreshTokenAsync(cancellationToken);

            if (!string.IsNullOrEmpty(newAccessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newAccessToken);
                response.Dispose();
                return await base.SendAsync(request, cancellationToken);
            }
        }
        finally
        {
            Semaphore.Release();
        }

        SignOut();
        return response;
    }

    private static bool IsTokenDifferent(HttpRequestMessage request, string? currentToken)
    {
        if (string.IsNullOrEmpty(currentToken)) return false;

        var sentToken = request.Headers.Authorization?.Parameter;
        return sentToken != currentToken;
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

        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (!(Application.Current?.Windows.Count > 0)) return;

            var services = Application.Current.Handler?.MauiContext?.Services;
            var loginPage = services?.GetService<LoginPageView>();

            if (loginPage != null)
                Application.Current.Windows[0].Page = new NavigationPage(loginPage);
        });
    }
}
