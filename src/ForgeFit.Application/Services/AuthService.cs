using ForgeFit.Application.Common.Exceptions.AuthExceptions;
using ForgeFit.Application.Common.Interfaces.Repositories;
using ForgeFit.Application.Common.Interfaces.Services;
using ForgeFit.Application.Common.Interfaces.Services.InfrastructureServices;
using ForgeFit.Application.DTOs.Auth;
using ForgeFit.Domain.Aggregates.UserAggregate;
using ForgeFit.Domain.Enums.ProfileEnums;
using ForgeFit.Domain.ValueObjects;
using ForgeFit.Domain.ValueObjects.UserValueObjects;

namespace ForgeFit.Application.Services;

public class AuthService(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IPasswordHasherService passwordHasherService,
    ITokenService tokenService,
    IUnitOfWork unitOfWork) : IAuthService
{
    public async Task<UserSignUpResponse> SignUpAsync(UserSignUpRequest request)
    {
        if (await userRepository.ExistsByEmailAsync(request.Email))
            throw new EmailAlreadyExistsException("Email already exists.");

        var passwordHash = passwordHasherService.HashPassword(request.Password);

        var email = Email.Create(request.Email);
        var avatarUri = string.IsNullOrWhiteSpace(request.Uri) ? null : new Uri(request.Uri);
        var dateOfBirth = DateOfBirth.Create(request.DateOfBirth);
        var weight = request.WeightUnit == WeightUnit.Kg ? Weight.FromKg(request.Weight) : Weight.FromLbs(request.Weight);
        var height = request.HeightUnit == HeightUnit.Cm ? Height.FromCm(request.Height) : Height.FromInches(request.Height);

        var userProfile = new UserProfile(
            request.Username,
            avatarUri,
            dateOfBirth,
            request.Gender,
            weight,
            height
        );

        var user = User.Create(
            userProfile,
            email,
            passwordHash
        );

        await userRepository.AddAsync(user);

        var accessTokenString = tokenService.GenerateAccessTokenString(user);
        var refreshToken = tokenService.GenerateRefreshToken(user.Id);

        await refreshTokenRepository.AddAsync(refreshToken);
        await unitOfWork.SaveChangesAsync();

        var refreshTokenString = refreshToken.Token;

        return new UserSignUpResponse(accessTokenString, refreshTokenString);
    }

    public async Task<UserSignInResponse> SignInAsync(UserSignInRequest request)
    {
        var user = await userRepository.GetByEmailAsync(request.Email);
        if (user == null) throw new InvalidCredentialsException("Invalid email.");

        if (!passwordHasherService.VerifyPassword(request.Password, user.PasswordHash))
            throw new InvalidCredentialsException("Invalid password.");

        var accessTokenString = tokenService.GenerateAccessTokenString(user);
        var refreshToken = tokenService.GenerateRefreshToken(user.Id);

        var existingToken = await refreshTokenRepository.GetByUserIdAsync(user.Id);
        if (existingToken != null)
            existingToken.Update(refreshToken);
        else
            await refreshTokenRepository.AddAsync(refreshToken);

        await unitOfWork.SaveChangesAsync();

        return new UserSignInResponse(accessTokenString, refreshToken.Token);
    }

    public async Task<CheckEmailResponse> CheckEmailAsync(CheckEmailRequest request)
    {
        if (await userRepository.ExistsByEmailAsync(request.Email))
            return new CheckEmailResponse(true);

        return new CheckEmailResponse(false);
    }

    public async Task<UserSignInResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var existingToken = await refreshTokenRepository.GetByTokenAsync(request.RefreshToken);
        if (existingToken is null || !existingToken.IsActive)
            throw new InvalidCredentialsException("Invalid or expired refresh token.");

        var user = await userRepository.GetByIdAsync(existingToken.UserId);
        if (user is null) throw new InvalidCredentialsException("User not found.");

        var newAccessTokenString = tokenService.GenerateAccessTokenString(user);
        var newRefreshToken = tokenService.GenerateRefreshToken(user.Id);

        existingToken.Update(newRefreshToken);
        await unitOfWork.SaveChangesAsync();

        return new UserSignInResponse(newAccessTokenString, newRefreshToken.Token);
    }
}
