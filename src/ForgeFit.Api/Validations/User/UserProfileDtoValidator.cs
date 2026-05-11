using FluentValidation;
using ForgeFit.Application.DTOs.User;
using ForgeFit.Domain.Constants;
using ForgeFit.Domain.Enums.ProfileEnums;
using ForgeFit.Domain.ValueObjects.UserValueObjects;

namespace ForgeFit.Api.Validations.User;

public class UserProfileDtoValidator : AbstractValidator<UserProfileDto>
{
    public UserProfileDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MaximumLength(DomainConstants.ValidationLimits.MaxUsernameLength).WithMessage($"Username must not exceed {DomainConstants.ValidationLimits.MaxUsernameLength} characters.");

        RuleFor(x => x.AvatarUrl)
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .When(x => !string.IsNullOrEmpty(x.AvatarUrl))
            .WithMessage("AvatarUrl must be an absolute URI.");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required.")
            .Must(date => {
                var today = DateTime.UtcNow;
                var earliestBirthDate = today.AddYears(-DomainConstants.ValidationLimits.MaxAgeYears);
                var latestBirthDate = today.AddYears(-DomainConstants.ValidationLimits.MinAgeYears);
                return date <= latestBirthDate && date >= earliestBirthDate;
            }).WithMessage($"You must be between {DomainConstants.ValidationLimits.MinAgeYears} and {DomainConstants.ValidationLimits.MaxAgeYears} years old.");

        RuleFor(x => x.Gender).IsInEnum();

        RuleFor(x => x.WeightUnit).IsInEnum();
        RuleFor(x => x.Weight)
            .InclusiveBetween(DomainConstants.ValidationLimits.MinWeightKg, DomainConstants.ValidationLimits.MaxWeightKg).When(x => x.WeightUnit == WeightUnit.Kg)
            .InclusiveBetween(DomainConstants.ValidationLimits.MinWeightLbs, DomainConstants.ValidationLimits.MaxWeightLbs).When(x => x.WeightUnit == WeightUnit.Lb);

        RuleFor(x => x.HeightUnit).IsInEnum();
        RuleFor(x => x.Height)
            .InclusiveBetween(DomainConstants.ValidationLimits.MinHeightCm, DomainConstants.ValidationLimits.MaxHeightCm).When(x => x.HeightUnit == HeightUnit.Cm)
            .InclusiveBetween(DomainConstants.ValidationLimits.MinHeightInches, DomainConstants.ValidationLimits.MaxHeightInches).When(x => x.HeightUnit == HeightUnit.Inch);
    }
}
