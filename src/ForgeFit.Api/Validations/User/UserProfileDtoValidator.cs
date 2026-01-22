using FluentValidation;
using ForgeFit.Application.DTOs.User;
using ForgeFit.Domain.Enums.ProfileEnums;
using ForgeFit.Domain.ValueObjects.UserValueObjects;

namespace ForgeFit.Api.Validations.User;

public class UserProfileDtoValidator : AbstractValidator<UserProfileDto>
{
    public UserProfileDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MaximumLength(20).WithMessage("Username must not exceed 20 characters.");

        RuleFor(x => x.AvatarUrl)
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .When(x => !string.IsNullOrEmpty(x.AvatarUrl))
            .WithMessage("AvatarUrl must be an absolute URI.");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required.")
            .LessThan(DateOfBirth.LatestBirthDate)
            .GreaterThan(DateOfBirth.EarliestBirthDate).WithMessage("You must be between 13 and 100 years old.");

        RuleFor(x => x.Gender).IsInEnum();

        RuleFor(x => x.WeightUnit).IsInEnum();
        RuleFor(x => x.Weight)
            .InclusiveBetween(30, 300).When(x => x.WeightUnit == WeightUnit.Kg)
            .InclusiveBetween(66, 660).When(x => x.WeightUnit == WeightUnit.Lb);

        RuleFor(x => x.HeightUnit).IsInEnum();
        RuleFor(x => x.Height)
            .InclusiveBetween(100, 250).When(x => x.HeightUnit == HeightUnit.Cm)
            .InclusiveBetween(40, 98).When(x => x.HeightUnit == HeightUnit.Inch);
    }
}
