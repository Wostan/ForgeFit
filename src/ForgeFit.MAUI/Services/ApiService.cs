using System.Net.Http.Json;
using ForgeFit.MAUI.Models;
using ForgeFit.MAUI.Resources.Strings;
// using ForgeFit.MAUI.Resources.Strings;
using ForgeFit.MAUI.Services.Interfaces;

namespace ForgeFit.MAUI.Services;

public class ApiService(HttpClient httpClient) : IApiService
{
    public async Task<ServiceResponse<T>> GetAsync<T>(string url)
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            return ServiceResponse<T>.Error(AppResources.NoInternetConnectionMessage);

        try
        {
            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<T>();
                return ServiceResponse<T>.Ok(data!);
            }

            var errorContent = await response.Content.ReadAsStringAsync();

            var message = !string.IsNullOrWhiteSpace(errorContent)
                ? errorContent
                : $"{AppResources.ServerErrorPrefix} {response.ReasonPhrase}";

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                message = AppResources.InvalidCredentialsMessage;
            else if ((int)response.StatusCode >= 500)
                message = AppResources.ServerUnavailableMessage;

            return ServiceResponse<T>.Error(message, (int)response.StatusCode);
        }
        catch (TaskCanceledException)
        {
            return ServiceResponse<T>.Error(AppResources.ConnectionTimedOutMessage);
        }
        catch (HttpRequestException)
        {
            return ServiceResponse<T>.Error(AppResources.ConnectionFailedMessage);
        }
        catch (Exception e)
        {
            Console.WriteLine($@"Critical Exception: {e}");
            return ServiceResponse<T>.Error(AppResources.UnexpectedErrorMessage);
        }
    }

    public async Task<ServiceResponse<TResponse>> PostAsync<TRequest, TResponse>(string url, TRequest request)
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            return ServiceResponse<TResponse>.Error(AppResources.NoInternetConnectionMessage);

        try
        {
            var response = await httpClient.PostAsJsonAsync(url, request);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<TResponse>();
                return ServiceResponse<TResponse>.Ok(data!);
            }

            var errorContent = await response.Content.ReadAsStringAsync();

            var message = !string.IsNullOrWhiteSpace(errorContent)
                ? errorContent
                : $"{AppResources.ServerErrorPrefix} {response.ReasonPhrase}";

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                message = AppResources.InvalidCredentialsMessage;
            else if ((int)response.StatusCode >= 500)
                message = AppResources.ServerUnavailableMessage;

            return ServiceResponse<TResponse>.Error(message, (int)response.StatusCode);
        }
        catch (TaskCanceledException)
        {
            return ServiceResponse<TResponse>.Error(AppResources.ConnectionTimedOutMessage);
        }
        catch (HttpRequestException)
        {
            return ServiceResponse<TResponse>.Error(AppResources.ConnectionFailedMessage);
        }
        catch (Exception e)
        {
            Console.WriteLine($@"Critical Exception: {e}");
            return ServiceResponse<TResponse>.Error(AppResources.UnexpectedErrorMessage);
        }
    }

    public async Task<ServiceResponse<TResponse?>> PutAsync<TRequest, TResponse>(string url, TRequest request)
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            return ServiceResponse<TResponse?>.Error(AppResources.NoInternetConnectionMessage);

        try
        {
            var response = await httpClient.PutAsJsonAsync(url, request);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<TResponse>();
                return ServiceResponse<TResponse?>.Ok(data!);
            }

            var errorContent = await response.Content.ReadAsStringAsync();

            var message = !string.IsNullOrWhiteSpace(errorContent)
                ? errorContent
                : $"{AppResources.ServerErrorPrefix} {response.ReasonPhrase}";

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                message = AppResources.InvalidCredentialsMessage;
            else if ((int)response.StatusCode >= 500)
                message = AppResources.ServerUnavailableMessage;

            return ServiceResponse<TResponse?>.Error(message, (int)response.StatusCode);
        }
        catch (TaskCanceledException)
        {
            return ServiceResponse<TResponse?>.Error(AppResources.ConnectionTimedOutMessage);
        }
        catch (HttpRequestException)
        {
            return ServiceResponse<TResponse?>.Error(AppResources.ConnectionFailedMessage);
        }
        catch (Exception e)
        {
            Console.WriteLine($@"Critical Exception: {e}");
            return ServiceResponse<TResponse?>.Error(AppResources.UnexpectedErrorMessage);
        }
    }

    public async Task<ServiceResponse<bool>> DeleteAsync(string url)
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            return ServiceResponse<bool>.Error(AppResources.NoInternetConnectionMessage);

        try
        {
            var response = await httpClient.DeleteAsync(url);

            if (response.IsSuccessStatusCode) return ServiceResponse<bool>.Ok(true);

            var errorContent = await response.Content.ReadAsStringAsync();

            var message = !string.IsNullOrWhiteSpace(errorContent)
                ? errorContent
                : $"{AppResources.ServerErrorPrefix} {response.ReasonPhrase}";

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                message = AppResources.InvalidCredentialsMessage;
            else if ((int)response.StatusCode >= 500)
                message = AppResources.ServerUnavailableMessage;

            return ServiceResponse<bool>.Error(message, (int)response.StatusCode);
        }
        catch (TaskCanceledException)
        {
            return ServiceResponse<bool>.Error(AppResources.ConnectionTimedOutMessage);
        }
        catch (HttpRequestException)
        {
            return ServiceResponse<bool>.Error(AppResources.ConnectionFailedMessage);
        }
        catch (Exception e)
        {
            Console.WriteLine($@"Critical Exception: {e}");
            return ServiceResponse<bool>.Error(AppResources.UnexpectedErrorMessage);
        }
    }
}
