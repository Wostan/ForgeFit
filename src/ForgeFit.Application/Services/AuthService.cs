using ForgeFit.Application.Common.Exceptions.AuthExceptions;
using ForgeFit.Application.Common.Interfaces.Repositories;
using ForgeFit.Application.Common.Interfaces.Services;
using ForgeFit.Application.Common.Interfaces.Services.InfrastructureServices;
using ForgeFit.Application.DTOs.Auth;
using ForgeFit.Domain.Aggregates.UserAggregate;
using ForgeFit.Domain.ValueObjects;
using ForgeFit.Domain.ValueObjects.UserValueObjects;
using MapsterMapper;

namespace ForgeFit.Application.Services;

public class AuthService(
    IUserRepository userRepository, 
    IUnitOfWork unitOfWork,
    IPasswordHasherService passwordHasherService,
    IMapper mapper
    ) : IAuthService
{
    public async Task<UserSignUpResponse> SignUpAsync(UserSignUpRequest request)
    { 
        if (await userRepository.ExistsByEmailAsync(request.Email))
        {
            throw new EmailAlreadyExistsException(request.Email);
        }
        
        var passwordHash = passwordHasherService.HashPassword(request.Password);

        var user = User.Create(
            new UserProfile(
                request.Username,
                string.IsNullOrWhiteSpace(request.Uri) ? null : new Uri(request.Uri),
                new DateOfBirth(request.DateOfBirth),
                request.Gender,
                new Weight(request.Weight, request.WeightUnit),
                new Height(request.Height, request.HeightUnit)
            ),
            new Email(request.Email),
            passwordHash
        );
        
        await userRepository.AddAsync(user);
        await unitOfWork.SaveChangesAsync(); 
        
        var response = mapper.Map<UserSignUpResponse>(user);
        
        return response;
    }

    public async Task<UserSignInResponse> SignInAsync(UserSignInRequest request)
    {
        var user = await userRepository.GetByEmailAsync(request.Email);
        if (user == null)
        {
            throw new InvalidCredentialsException("Invalid email.");
        }

        if (!passwordHasherService.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new InvalidCredentialsException("Invalid password.");
        }
        
        var response = mapper.Map<UserSignInResponse>(user);
        
        return response;
    }

    public async Task<CheckEmailResponse> CheckEmailAsync(CheckEmailRequest request)
    {
        if(await userRepository.ExistsByEmailAsync(request.Email))
            return new CheckEmailResponse(true);
        
        return new CheckEmailResponse(false);
    }
}