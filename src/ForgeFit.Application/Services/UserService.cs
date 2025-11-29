using ForgeFit.Application.Common.Exceptions;
using ForgeFit.Application.Common.Interfaces.Repositories;
using ForgeFit.Application.Common.Interfaces.Services;
using ForgeFit.Application.Common.Interfaces.Services.InfrastructureServices;
using ForgeFit.Application.DTOs.User;
using ForgeFit.Domain.ValueObjects.UserValueObjects;
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

        if (user == null)
        {
            throw new NotFoundException("User not found.");
        }
        
        return mapper.Map<UserProfileDto>(user.UserProfile);
    }

    public async Task<UserProfileDto> UpdateProfileByIdAsync(Guid userId, UserProfileDto profile)
    {
        var user = await userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        var updatedUserProfile = mapper.Map<UserProfile>(profile);
        
        user.UpdateUserProfile(updatedUserProfile);
        await unitOfWork.SaveChangesAsync();
        
        return mapper.Map<UserProfileDto>(user.UserProfile);
    }

    public async Task ChangePasswordByIdAsync(Guid userId, ChangePasswordRequest password)
    {
        var user = await userRepository.GetByIdAsync(userId);
        if (user is null)
        {
            throw new NotFoundException("User not found");
        }
        
        var currentPasswordHash = user.PasswordHash;
        var isPasswordValid = passwordHasherService.VerifyPassword(password.CurrentPassword, currentPasswordHash);
        
        if (!isPasswordValid)
        {
            throw new BadRequestException("Current password is incorrect");
        }
        
        var newPasswordHash = passwordHasherService.HashPassword(password.NewPassword);
        
        user.UpdatePasswordHash(newPasswordHash);
        await unitOfWork.SaveChangesAsync();
    }
}