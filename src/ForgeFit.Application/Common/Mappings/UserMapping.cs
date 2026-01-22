using ForgeFit.Application.DTOs.User;
using ForgeFit.Domain.ValueObjects;
using ForgeFit.Domain.ValueObjects.UserValueObjects;
using Mapster;

namespace ForgeFit.Application.Common.Mappings;

public class UserMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<UserProfile, UserProfileDto>()
            .Map(dest => dest.Username, src => src.Username)
            .Map(dest => dest.AvatarUrl, src => src.AvatarUrl != null ? src.AvatarUrl.ToString() : null)
            .Map(dest => dest.DateOfBirth, src => src.DateOfBirth.Value)
            .Map(dest => dest.Gender, src => src.Gender)
            .Map(dest => dest.Weight, src => src.Weight.Value)
            .Map(dest => dest.WeightUnit, src => src.Weight.Unit)
            .Map(dest => dest.Height, src => src.Height.Value)
            .Map(dest => dest.HeightUnit, src => src.Height.Unit);

        config.NewConfig<UserProfileDto, UserProfile>()
            .MapWith(src => new UserProfile(
                src.Username,
                string.IsNullOrWhiteSpace(src.AvatarUrl) ? null : new Uri(src.AvatarUrl),
                new DateOfBirth(src.DateOfBirth),
                src.Gender,
                new Weight(src.Weight, src.WeightUnit),
                new Height(src.Height, src.HeightUnit)
            ));
    }
}
