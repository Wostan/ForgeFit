using FluentValidation;
using ForgeFit.Application.DTOs.User;
using ForgeFit.Domain.Constants;

namespace ForgeFit.Api.Validations.User;

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Current password is required.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required.")
            .MinimumLength(DomainConstants.ValidationLimits.MinPasswordLength).WithMessage($"Password must be at least {DomainConstants.ValidationLimits.MinPasswordLength} characters long.")
            .NotEqual(x => x.CurrentPassword).WithMessage("New password must be different from the current password.");
    }
}
