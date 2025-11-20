using ForgeFit.Application.DTOs.Auth;
using ForgeFit.Domain.Aggregates.UserAggregate;
using ForgeFit.Domain.ValueObjects;
using ForgeFit.Domain.ValueObjects.UserValueObjects;
using Mapster;

namespace ForgeFit.Application.Common.Mappings;

public class AuthMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<UserSignUpRequest, User>()
            .MapWith(src => User.Create(
                new UserProfile(
                    src.Username,
                    new Uri(src.Uri!),
                    new DateOfBirth(src.DateOfBirth),
                    src.Gender,
                    new Weight(src.Weight, src.WeightUnit),
                    new Height(src.Height, src.HeightUnit)
                ),
                
                new Email(src.Email),
                
                ""
            ));


        config.NewConfig<User, UserSignUpResponse>()
            .Map(dest => dest.Id, src => src.Id.ToString())
            .Map(dest => dest.Email, src => src.Email.Value)
            .Map(dest => dest.Username, src => src.UserProfile.Username)
            .Map(dest => dest.AvatarUri, src => src.UserProfile.AvatarUrl);

        config.NewConfig<User, UserSignInResponse>()
            .Map(dest => dest.Id, src => src.Id.ToString())
            .Map(dest => dest.Email, src => src.Email.Value);
    }
}