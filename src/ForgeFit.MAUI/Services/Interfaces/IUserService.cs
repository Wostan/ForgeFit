using ForgeFit.MAUI.Models;
using ForgeFit.MAUI.Models.DTOs.User;

namespace ForgeFit.MAUI.Services.Interfaces;

public interface IUserService
{
    Task<ServiceResponse<UserProfileDto?>> GetProfileAsync(CancellationToken cancellationToken);

    Task<ServiceResponse<UserProfileDto?>> UpdateProfileAsync(UserProfileDto userProfileDto,
        CancellationToken cancellationToken);

    Task<ServiceResponse<string?>> ChangePasswordAsync(ChangePasswordRequest changePasswordRequest,
        CancellationToken cancellationToken);
}
