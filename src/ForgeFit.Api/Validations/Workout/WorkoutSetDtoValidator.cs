using FluentValidation;
using ForgeFit.Application.DTOs.Workout;
using ForgeFit.Domain.Constants;

namespace ForgeFit.Api.Validations.Workout;

public class WorkoutSetDtoValidator : AbstractValidator<WorkoutSetDto>
{
    public WorkoutSetDtoValidator()
    {
        RuleFor(x => x.Order)
            .GreaterThan(0).WithMessage("Order must be greater than 0.");

        RuleFor(x => x.Reps)
            .InclusiveBetween(1, DomainConstants.ValidationLimits.MaxRepsPerSet).WithMessage($"Reps must be between 1 and {DomainConstants.ValidationLimits.MaxRepsPerSet}.");

        RuleFor(x => x.RestTime)
            .LessThanOrEqualTo(TimeSpan.FromMinutes(DomainConstants.ValidationLimits.MaxRestTimeMinutes)).WithMessage($"Rest time must be less or equal to {DomainConstants.ValidationLimits.MaxRestTimeMinutes} minutes.");

        RuleFor(x => x.Weight)
            .NotNull();

        RuleFor(x => x.Weight)
            .GreaterThanOrEqualTo(0)
            .LessThan(DomainConstants.ValidationLimits.MaxWorkoutWeightKg).WithMessage($"Weight seems unreasonably high (>{DomainConstants.ValidationLimits.MaxWorkoutWeightKg}).");

        RuleFor(x => x.WeightUnit)
            .IsInEnum();
    }
}
