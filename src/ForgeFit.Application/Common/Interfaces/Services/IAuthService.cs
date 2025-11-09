using ForgeFit.Application.DTOs.Auth;

namespace ForgeFit.Application.Common.Interfaces.Services;

public interface IAuthService
{
    Task<UserSignUpResponse> SignUpAsync(UserSignUpRequest request); 
    Task<UserSignInResponse> SignInAsync(UserSignInRequest request);
    Task<CheckEmailResponse> CheckEmailAsync(CheckEmailRequest request);
}