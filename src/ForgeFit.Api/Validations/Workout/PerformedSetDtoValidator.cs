using FluentValidation;
using ForgeFit.Application.DTOs.Workout;
using ForgeFit.Domain.Constants;

namespace ForgeFit.Api.Validations.Workout;

public class PerformedSetDtoValidator : AbstractValidator<PerformedSetDto>
{
    public PerformedSetDtoValidator()
    {
        RuleFor(x => x.Order)
            .GreaterThanOrEqualTo(0).WithMessage("Order cannot be negative.");

        RuleFor(x => x.Reps)
            .InclusiveBetween(1, DomainConstants.ValidationLimits.MaxRepsPerSet).WithMessage($"Reps must be between 1 and {DomainConstants.ValidationLimits.MaxRepsPerSet}.");

        RuleFor(x => x.Weight)
            .GreaterThanOrEqualTo(0)
            .LessThan(DomainConstants.ValidationLimits.MaxWorkoutWeightKg).WithMessage($"Weight seems unreasonably high (>{DomainConstants.ValidationLimits.MaxWorkoutWeightKg}).");

        RuleFor(x => x.WeightUnit)
            .IsInEnum();
    }
}
