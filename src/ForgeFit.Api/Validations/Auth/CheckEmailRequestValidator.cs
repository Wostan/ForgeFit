using FluentValidation;
using ForgeFit.Application.DTOs.Auth;

namespace ForgeFit.Api.Validations.Auth;

public class CheckEmailRequestValidator : AbstractValidator<CheckEmailRequest>
{
    public CheckEmailRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");
    }
}
