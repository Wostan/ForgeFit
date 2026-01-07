using ForgeFit.MAUI.Models;

namespace ForgeFit.MAUI.Services.Interfaces;

public interface IApiService
{
    Task<ServiceResponse<T>> GetAsync<T>(string url, CancellationToken cancellationToken = default);

    Task<ServiceResponse<TResponse>> PostAsync<TRequest, TResponse>(string url, TRequest request,
        CancellationToken cancellationToken = default);

    Task<ServiceResponse<TResponse?>> PutAsync<TRequest, TResponse>(string url, TRequest request,
        CancellationToken cancellationToken = default);

    Task<ServiceResponse<bool>> DeleteAsync(string url, CancellationToken cancellationToken = default);
}
