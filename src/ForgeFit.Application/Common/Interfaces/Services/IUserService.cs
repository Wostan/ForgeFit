using ForgeFit.Application.DTOs.User;

namespace ForgeFit.Application.Common.Interfaces.Services;

public interface IUserService
{
    Task<UserProfileDto> GetProfileByIdAsync(Guid id);
    Task<UserProfileDto> UpdateProfileByIdAsync(Guid userId, UserProfileDto profile);
    Task ChangePasswordByIdAsync(Guid userId, ChangePasswordRequest password);
}