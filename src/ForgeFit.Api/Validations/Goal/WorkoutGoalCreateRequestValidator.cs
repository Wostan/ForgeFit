using FluentValidation;
using ForgeFit.Application.DTOs.Goal;
using ForgeFit.Domain.Constants;

namespace ForgeFit.Api.Validations.Goal;

public class WorkoutGoalCreateRequestValidator : AbstractValidator<WorkoutGoalCreateRequest>
{
    public WorkoutGoalCreateRequestValidator()
    {
        RuleFor(x => x.WorkoutsPerWeek)
            .InclusiveBetween(DomainConstants.ValidationLimits.MinWorkoutsPerWeek, DomainConstants.ValidationLimits.MaxWorkoutsPerWeek).WithMessage($"Workouts per week must be between {DomainConstants.ValidationLimits.MinWorkoutsPerWeek} and {DomainConstants.ValidationLimits.MaxWorkoutsPerWeek}.");

        RuleFor(x => x.Duration)
            .GreaterThanOrEqualTo(TimeSpan.FromMinutes(DomainConstants.ValidationLimits.MinWorkoutDurationMinutes))
            .LessThanOrEqualTo(TimeSpan.FromHours(DomainConstants.ValidationLimits.MaxWorkoutDurationHours))
            .WithMessage($"Workout duration goal must be between {DomainConstants.ValidationLimits.MinWorkoutDurationMinutes} minutes and {DomainConstants.ValidationLimits.MaxWorkoutDurationHours} hours.");

        RuleFor(x => x.WorkoutType).IsInEnum();
    }
}
