namespace ForgeFit.Application.Common.Interfaces.Services.InfrastructureServices;

public interface IFatSecretTokenService
{
    Task<string> GetAccessTokenAsync();
}