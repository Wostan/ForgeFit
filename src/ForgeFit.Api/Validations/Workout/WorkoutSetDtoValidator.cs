using FluentValidation;
using ForgeFit.Application.DTOs.Workout;

namespace ForgeFit.Api.Validations.Workout;

public class WorkoutSetDtoValidator : AbstractValidator<WorkoutSetDto>
{
    public WorkoutSetDtoValidator()
    {
        RuleFor(x => x.Order)
            .GreaterThan(0).WithMessage("Order must be greater than 0.");

        RuleFor(x => x.Reps)
            .InclusiveBetween(0, 100).WithMessage("Reps must be between 0 and 100.");

        RuleFor(x => x.RestTime)
            .LessThan(TimeSpan.FromMinutes(10)).WithMessage("Rest time must be less than 10 minutes.");

        RuleFor(x => x.Weight)
            .NotNull();

        RuleFor(x => x.Weight.Value)
            .GreaterThanOrEqualTo(0)
            .LessThan(1500).WithMessage("Weight seems unreasonably high (>1500).");

        RuleFor(x => x.Weight.Unit)
            .IsInEnum();
    }
}
