using FluentValidation;
using ForgeFit.Application.DTOs.Workout;
using ForgeFit.Domain.Constants;

namespace ForgeFit.Api.Validations.Workout;

public class WorkoutEntryDtoValidator : AbstractValidator<WorkoutEntryDto>
{
    public WorkoutEntryDtoValidator()
    {
        RuleFor(x => x.End)
            .GreaterThan(x => x.Start).WithMessage("End time must be after start time.");

        RuleFor(x => x)
            .Must(x => x.End - x.Start >= TimeSpan.FromMinutes(DomainConstants.ValidationLimits.MinWorkoutDurationMinutes))
            .WithMessage($"Workout looks too short (< {DomainConstants.ValidationLimits.MinWorkoutDurationMinutes} min).")
            .Must(x => x.End - x.Start <= TimeSpan.FromHours(DomainConstants.ValidationLimits.MaxWorkoutDurationHours))
            .WithMessage($"Workout looks too long (> {DomainConstants.ValidationLimits.MaxWorkoutDurationHours} hours).");

        RuleFor(x => x.PerformedExercises)
            .NotNull()
            .Must(x => x.Count > 0).WithMessage("Workout entry must contain exercises.");

        RuleForEach(x => x.PerformedExercises).SetValidator(new PerformedExerciseDtoValidator());
    }
}
