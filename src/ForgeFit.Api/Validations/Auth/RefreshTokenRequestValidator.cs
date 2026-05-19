using FluentValidation;
using ForgeFit.Application.DTOs.Auth;
using ForgeFit.Domain.Constants;

namespace ForgeFit.Api.Validations.Auth;

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required.")
            .MaximumLength(DomainConstants.ValidationLimits.MaxRefreshTokenLength).WithMessage($"Refresh token cannot exceed {DomainConstants.ValidationLimits.MaxRefreshTokenLength} characters.");
    }
}
