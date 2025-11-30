using FluentValidation;
using ForgeFit.Application.DTOs.Workout;

namespace ForgeFit.Api.Validations.Workout;

public class WorkoutEntryDtoValidator : AbstractValidator<WorkoutEntryDto>
{
    public WorkoutEntryDtoValidator()
    {
        RuleFor(x => x.End)
            .GreaterThan(x => x.Start).WithMessage("End time must be after start time.");
        
        RuleFor(x => x)
            .Must(x => x.End - x.Start >= TimeSpan.FromMinutes(10))
            .WithMessage("Workout looks too short (< 10 min).")
            .Must(x => x.End - x.Start <= TimeSpan.FromHours(5))
            .WithMessage("Workout looks too long (> 5 hours).");

        RuleFor(x => x.PerformedExercises)
            .NotNull()
            .Must(x => x.Count > 0).WithMessage("Workout entry must contain exercises.");

        RuleForEach(x => x.PerformedExercises).SetValidator(new PerformedExerciseDtoValidator());
    }
}