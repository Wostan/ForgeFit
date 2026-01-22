using FluentValidation;
using ForgeFit.Application.DTOs.Auth;
using ForgeFit.Domain.Enums.ProfileEnums;

namespace ForgeFit.Api.Validations.Auth;

public class UserSignUpRequestValidator : AbstractValidator<UserSignUpRequest>
{
    public UserSignUpRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MaximumLength(20).WithMessage("Username must not exceed 20 characters.");

        RuleFor(x => x.Uri)
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .When(x => !string.IsNullOrEmpty(x.Uri))
            .WithMessage("Avatar URI must be absolute.");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty()
            .LessThan(DateTime.UtcNow.AddYears(-13)).WithMessage("You must be at least 13 years old.")
            .GreaterThan(DateTime.UtcNow.AddYears(-100)).WithMessage("Please enter a valid date of birth.");

        RuleFor(x => x.Gender).IsInEnum();

        RuleFor(x => x.WeightUnit).IsInEnum();
        RuleFor(x => x.Weight)
            .GreaterThan(0).WithMessage("Weight must be greater than 0.")
            .InclusiveBetween(30, 300)
            .When(x => x.WeightUnit == WeightUnit.Kg)
            .WithMessage("Weight must be between 30kg and 300kg.")
            .InclusiveBetween(66, 660)
            .When(x => x.WeightUnit == WeightUnit.Lb)
            .WithMessage("Weight must be between 66lb and 660lb.");

        RuleFor(x => x.HeightUnit).IsInEnum();
        RuleFor(x => x.Height)
            .GreaterThan(0).WithMessage("Height must be greater than 0.")
            .InclusiveBetween(100, 250)
            .When(x => x.HeightUnit == HeightUnit.Cm)
            .WithMessage("Height must be between 100cm and 250cm.")
            .InclusiveBetween(40, 98)
            .When(x => x.HeightUnit == HeightUnit.Inch)
            .WithMessage("Height must be between 40 and 98 inches.");
    }
}
