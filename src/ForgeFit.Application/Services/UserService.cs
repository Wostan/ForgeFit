using ForgeFit.Application.Common.Exceptions;
using ForgeFit.Application.Common.Interfaces.Repositories;
using ForgeFit.Application.Common.Interfaces.Services;
using ForgeFit.Application.Common.Interfaces.Services.InfrastructureServices;
using ForgeFit.Application.DTOs.User;
using ForgeFit.Domain.ValueObjects.UserValueObjects;
using ForgeFit.Domain.Enums.ProfileEnums;
using ForgeFit.Domain.ValueObjects;
using MapsterMapper;

namespace ForgeFit.Application.Services;

public class UserService(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasherService passwordHasherService,
    IMapper mapper) : IUserService
{
    public async Task<UserProfileDto> GetProfileByIdAsync(Guid id)
    {
        var user = await userRepository.GetByIdAsync(id);

        if (user == null) throw new NotFoundException("User not found.");

        return mapper.Map<UserProfileDto>(user.UserProfile);
    }

    public async Task<UserProfileDto> UpdateProfileByIdAsync(Guid userId, UserProfileDto profile)
    {
        var user = await userRepository.GetByIdAsync(userId);
        if (user == null) throw new NotFoundException("User not found");

        var avatarUri = string.IsNullOrWhiteSpace(profile.AvatarUrl) ? null : new Uri(profile.AvatarUrl);
        var dateOfBirth = DateOfBirth.Create(profile.DateOfBirth);
        var weight = profile.WeightUnit == WeightUnit.Kg ? Weight.FromKg(profile.Weight) : Weight.FromLbs(profile.Weight);
        var height = profile.HeightUnit == HeightUnit.Cm ? Height.FromCm(profile.Height) : Height.FromInches(profile.Height);

        var updatedUserProfile = new UserProfile(
            profile.Username,
            avatarUri,
            dateOfBirth,
            profile.Gender,
            weight,
            height
        );

        user.UpdateUserProfile(updatedUserProfile);
        await unitOfWork.SaveChangesAsync();

        return mapper.Map<UserProfileDto>(user.UserProfile);
    }

    public async Task ChangePasswordByIdAsync(Guid userId, ChangePasswordRequest request)
    {
        var user = await userRepository.GetByIdAsync(userId);
        if (user is null) throw new NotFoundException("User not found");

        var currentPasswordHash = user.PasswordHash;
        var isPasswordValid = passwordHasherService.VerifyPassword(request.CurrentPassword, currentPasswordHash);

        if (!isPasswordValid) throw new BadRequestException("Current password is incorrect");

        var newPasswordHash = passwordHasherService.HashPassword(request.NewPassword);

        user.UpdatePasswordHash(newPasswordHash);
        await unitOfWork.SaveChangesAsync();
    }
}
