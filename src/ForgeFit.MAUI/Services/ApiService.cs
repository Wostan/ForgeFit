using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using ForgeFit.MAUI.Models;
using ForgeFit.MAUI.Resources.Strings;
using ForgeFit.MAUI.Services.Interfaces;

namespace ForgeFit.MAUI.Services;

public class ApiService(HttpClient httpClient) : IApiService
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task<ServiceResponse<T>> GetAsync<T>(string url, CancellationToken cancellationToken = default)
    {
        return await SendRequestAsync<T>(ct => httpClient.GetAsync(url, ct), cancellationToken);
    }

    public async Task<ServiceResponse<TResponse>> PostAsync<TRequest, TResponse>(string url, TRequest request,
        CancellationToken cancellationToken = default)
    {
        return await SendRequestAsync<TResponse>(ct => httpClient.PostAsJsonAsync(url, request, _jsonOptions, ct),
            cancellationToken);
    }

    public async Task<ServiceResponse<TResponse?>> PutAsync<TRequest, TResponse>(string url, TRequest request,
        CancellationToken cancellationToken = default)
    {
        return await SendRequestAsync<TResponse?>(ct => httpClient.PutAsJsonAsync(url, request, _jsonOptions, ct),
            cancellationToken);
    }

    public async Task<ServiceResponse<bool>> DeleteAsync(string url, CancellationToken cancellationToken = default)
    {
        return await SendRequestAsync<bool>(async ct => await httpClient.DeleteAsync(url, ct), cancellationToken);
    }

    private async Task<ServiceResponse<T>> SendRequestAsync<T>(
        Func<CancellationToken, Task<HttpResponseMessage>> requestFunc, CancellationToken cancellationToken)
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            return ServiceResponse<T>.Error(AppResources.NoInternetConnectionMessage);

        try
        {
            var response = await requestFunc(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    return ServiceResponse<T>.Ok(default!);

                var data = await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
                return ServiceResponse<T>.Ok(data!);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return ServiceResponse<T>.Error(AppResources.InvalidCredentialsMessage, 401);

            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var message = !string.IsNullOrWhiteSpace(errorContent)
                ? errorContent
                : $"{AppResources.ServerErrorPrefix} {response.ReasonPhrase}";

            if ((int)response.StatusCode >= 500)
                message = AppResources.ServerUnavailableMessage;

            return ServiceResponse<T>.Error(message, (int)response.StatusCode);
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"JSON Parse Error: {ex}");
            return ServiceResponse<T>.Error("Data format error.");
        }
        catch (TaskCanceledException)
        {
            return cancellationToken.IsCancellationRequested
                ? throw new OperationCanceledException(cancellationToken)
                : ServiceResponse<T>.Error(AppResources.ConnectionTimedOutMessage);
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
