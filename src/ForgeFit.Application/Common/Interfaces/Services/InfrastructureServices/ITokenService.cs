using ForgeFit.Domain.Aggregates.UserAggregate;

namespace ForgeFit.Application.Common.Interfaces.Services.InfrastructureServices;

public interface ITokenService
{
    string GenerateAccessTokenString(User user);
    RefreshToken GenerateRefreshToken(Guid userId);
}
