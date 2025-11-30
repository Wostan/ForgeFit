using FluentValidation;
using ForgeFit.Application.DTOs.Goal;

namespace ForgeFit.Api.Validations.Goal;

public class WorkoutGoalCreateRequestValidator : AbstractValidator<WorkoutGoalCreateRequest>
{
    public WorkoutGoalCreateRequestValidator()
    {
        RuleFor(x => x.WorkoutsPerWeek)
            .InclusiveBetween(0, 7).WithMessage("Workouts per week must be between 0 and 7.");
        
        RuleFor(x => x.Duration)
            .GreaterThan(TimeSpan.FromMinutes(5))
            .LessThan(TimeSpan.FromHours(5))
            .WithMessage("Workout duration goal must be between 5 minutes and 5 hours.");
        
        RuleFor(x => x.WorkoutType).IsInEnum();
    }
}