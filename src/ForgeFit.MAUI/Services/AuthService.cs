using ForgeFit.MAUI.Constants;
using ForgeFit.MAUI.Models;
using ForgeFit.MAUI.Models.DTOs.Auth;
using ForgeFit.MAUI.Services.Interfaces;

namespace ForgeFit.MAUI.Services;

public class AuthService(IApiService apiService) : IAuthService
{ 
    public async Task<ServiceResponse<bool>> SignUpAsync(UserSignUpRequest request)
    {
        var response = 
            await apiService.PostAsync<UserSignUpRequest, UserSignUpResponse>("/api/auth/sign-up", request);
        
        if (response is not { Success: true, Data: not null })
            return ServiceResponse<bool>.Error(response.Message, response.StatusCode);
        
        await SecureStorage.SetAsync(AuthConstants.AccessToken, response.Data.AccessToken);
        await SecureStorage.SetAsync(AuthConstants.RefreshToken, response.Data.RefreshToken);
            
        return ServiceResponse<bool>.Ok(true);
    }

    public async Task<ServiceResponse<bool>> SignInAsync(UserSignInRequest request)
    {
        var response = 
            await apiService.PostAsync<UserSignInRequest, UserSignInResponse>("/api/auth/sign-in", request);
        
        if (response is not { Success: true, Data: not null })
            return ServiceResponse<bool>.Error(response.Message, response.StatusCode);
        
        await SecureStorage.SetAsync(AuthConstants.AccessToken, response.Data.AccessToken);
        await SecureStorage.SetAsync(AuthConstants.RefreshToken, response.Data.RefreshToken);
            
        return ServiceResponse<bool>.Ok(true);
    }

    public void SignOut()
    {
        SecureStorage.Remove(AuthConstants.AccessToken);
        SecureStorage.Remove(AuthConstants.RefreshToken);
    }

    public async Task<bool> IsAuthenticatedAsync()
    { 
        var token = await SecureStorage.GetAsync(AuthConstants.AccessToken);
        return !string.IsNullOrEmpty(token);
    }

    public async Task<ServiceResponse<bool>> CheckEmailAsync(string email)
    {
        var response = 
            await apiService.PostAsync<CheckEmailRequest, CheckEmailResponse>("/api/auth/check-email", new CheckEmailRequest(email));

        if (response is not { Success: true, Data: not null })
            return ServiceResponse<bool>.Error(response.Message, response.StatusCode);
        
        return ServiceResponse<bool>.Ok(response.Data.IsTaken);
    }
}
