using ForgeFit.MAUI.Models;
using ForgeFit.MAUI.Models.DTOs.User;
using ForgeFit.MAUI.Services.Interfaces;

namespace ForgeFit.MAUI.Services;

public class UserService(IApiService apiService) : IUserService
{
    public async Task<ServiceResponse<UserProfileDto?>> GetProfileAsync(CancellationToken cancellationToken)
    {
        return await apiService.GetAsync<UserProfileDto?>("api/user/profile", cancellationToken);
    }

    public async Task<ServiceResponse<UserProfileDto?>> UpdateProfileAsync(UserProfileDto userProfileDto,
        CancellationToken cancellationToken)
    {
        return await apiService.PutAsync<UserProfileDto, UserProfileDto?>("api/user/profile", userProfileDto,
            cancellationToken);
    }

    public async Task<ServiceResponse<string?>> ChangePasswordAsync(ChangePasswordRequest changePasswordRequest,
        CancellationToken cancellationToken)
    {
        return await apiService.PutAsync<ChangePasswordRequest, string?>("api/user/change-password",
            changePasswordRequest, cancellationToken);
    }
}
