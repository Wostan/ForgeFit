using ForgeFit.MAUI.Models;
using ForgeFit.MAUI.Models.DTOs.Auth;

namespace ForgeFit.MAUI.Services.Interfaces;

public interface IAuthService
{
    Task<ServiceResponse<bool>> SignUpAsync(UserSignUpRequest request);
    Task<ServiceResponse<bool>> SignInAsync(UserSignInRequest request);
    void SignOut();
    Task<bool> IsAuthenticatedAsync();
    Task<ServiceResponse<bool>> CheckEmailAsync(string email);
}
