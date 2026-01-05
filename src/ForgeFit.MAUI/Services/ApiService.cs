using System.Net.Http.Json;
using ForgeFit.MAUI.Models;
using ForgeFit.MAUI.Resources.Strings;
using ForgeFit.MAUI.Services.Interfaces;

namespace ForgeFit.MAUI.Services;

public class ApiService(HttpClient httpClient) : IApiService
{
    public async Task<ServiceResponse<T>> GetAsync<T>(string url)
    {
        return await SendRequestAsync<T>(() => httpClient.GetAsync(url));
    }

    public async Task<ServiceResponse<TResponse>> PostAsync<TRequest, TResponse>(string url, TRequest request)
    {
        return await SendRequestAsync<TResponse>(() => httpClient.PostAsJsonAsync(url, request));
    }

    public async Task<ServiceResponse<TResponse?>> PutAsync<TRequest, TResponse>(string url, TRequest request)
    {
        return await SendRequestAsync<TResponse?>(() => httpClient.PutAsJsonAsync(url, request));
    }

    public async Task<ServiceResponse<bool>> DeleteAsync(string url)
    {
        return await SendRequestAsync<bool>(async () =>
        {
            var response = await httpClient.DeleteAsync(url);
            return response;
        });
    }

    private static async Task<ServiceResponse<T>> SendRequestAsync<T>(Func<Task<HttpResponseMessage>> requestFunc)
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            return ServiceResponse<T>.Error(AppResources.NoInternetConnectionMessage);

        try
        {
            var response = await requestFunc();

            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    return ServiceResponse<T>.Ok(default!);

                var data = await response.Content.ReadFromJsonAsync<T>();
                return ServiceResponse<T>.Ok(data!);
            }
            
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return ServiceResponse<T>.Error(AppResources.InvalidCredentialsMessage, 401);
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            var message = !string.IsNullOrWhiteSpace(errorContent)
                ? errorContent
                : $"{AppResources.ServerErrorPrefix} {response.ReasonPhrase}";

            if ((int)response.StatusCode >= 500)
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
}
