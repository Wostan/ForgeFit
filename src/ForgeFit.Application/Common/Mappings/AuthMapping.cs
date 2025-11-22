using ForgeFit.Application.DTOs.Auth;
using ForgeFit.Domain.Aggregates.UserAggregate;
using Mapster;

namespace ForgeFit.Application.Common.Mappings;

public class AuthMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<User, UserSignUpResponse>()
            .Map(dest => dest.Id, src => src.Id.ToString())
            .Map(dest => dest.Email, src => src.Email.Value)
            .Map(dest => dest.Username, src => src.UserProfile.Username)
            .Map(dest => dest.AvatarUri, src => src.UserProfile.AvatarUrl);
    }
}