using ForgeFit.Application.DTOs.User;
using ForgeFit.Domain.Enums.ProfileEnums;
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
    }
}
