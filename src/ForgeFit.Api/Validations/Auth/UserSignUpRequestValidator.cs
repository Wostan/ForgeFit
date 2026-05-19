using FluentValidation;
using ForgeFit.Application.DTOs.Auth;
using ForgeFit.Domain.Constants;
using ForgeFit.Domain.Enums.ProfileEnums;

namespace ForgeFit.Api.Validations.Auth;

public class UserSignUpRequestValidator : AbstractValidator<UserSignUpRequest>
{
    public UserSignUpRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(DomainConstants.ValidationLimits.MaxEmailLength).WithMessage($"Email must not exceed {DomainConstants.ValidationLimits.MaxEmailLength} characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(DomainConstants.ValidationLimits.MinPasswordLength).WithMessage($"Password must be at least {DomainConstants.ValidationLimits.MinPasswordLength} characters long.");

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MaximumLength(DomainConstants.ValidationLimits.MaxUsernameLength).WithMessage($"Username must not exceed {DomainConstants.ValidationLimits.MaxUsernameLength} characters.");

        RuleFor(x => x.Uri)
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .When(x => !string.IsNullOrEmpty(x.Uri)).WithMessage("Avatar URI must be absolute.")
            .MaximumLength(DomainConstants.ValidationLimits.MaxAvatarUrlLength).WithMessage($"Avatar URL cannot exceed {DomainConstants.ValidationLimits.MaxAvatarUrlLength} characters.");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty()
            .LessThan(DateTime.UtcNow.AddYears(-DomainConstants.ValidationLimits.MinAgeYears)).WithMessage($"You must be at least {DomainConstants.ValidationLimits.MinAgeYears} years old.")
            .GreaterThan(DateTime.UtcNow.AddYears(-DomainConstants.ValidationLimits.MaxAgeYears)).WithMessage("Please enter a valid date of birth.");

        RuleFor(x => x.Gender).IsInEnum();

        RuleFor(x => x.WeightUnit).IsInEnum();
        RuleFor(x => x.Weight)
            .GreaterThan(0).WithMessage("Weight must be greater than 0.")
            .InclusiveBetween(DomainConstants.ValidationLimits.MinWeightKg, DomainConstants.ValidationLimits.MaxWeightKg)
            .When(x => x.WeightUnit == WeightUnit.Kg)
            .WithMessage($"Weight must be between {DomainConstants.ValidationLimits.MinWeightKg}kg and {DomainConstants.ValidationLimits.MaxWeightKg}kg.")
            .InclusiveBetween(DomainConstants.ValidationLimits.MinWeightLbs, DomainConstants.ValidationLimits.MaxWeightLbs)
            .When(x => x.WeightUnit == WeightUnit.Lb)
            .WithMessage($"Weight must be between {DomainConstants.ValidationLimits.MinWeightLbs}lb and {DomainConstants.ValidationLimits.MaxWeightLbs}lb.");

        RuleFor(x => x.HeightUnit).IsInEnum();
        RuleFor(x => x.Height)
            .GreaterThan(0).WithMessage("Height must be greater than 0.")
            .InclusiveBetween(DomainConstants.ValidationLimits.MinHeightCm, DomainConstants.ValidationLimits.MaxHeightCm)
            .When(x => x.HeightUnit == HeightUnit.Cm)
            .WithMessage($"Height must be between {DomainConstants.ValidationLimits.MinHeightCm}cm and {DomainConstants.ValidationLimits.MaxHeightCm}cm.")
            .InclusiveBetween(DomainConstants.ValidationLimits.MinHeightInches, DomainConstants.ValidationLimits.MaxHeightInches)
            .When(x => x.HeightUnit == HeightUnit.Inch)
            .WithMessage($"Height must be between {DomainConstants.ValidationLimits.MinHeightInches} and {DomainConstants.ValidationLimits.MaxHeightInches} inches.");
    }
}
