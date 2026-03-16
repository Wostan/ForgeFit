using FluentValidation;
using ForgeFit.Application.DTOs.Workout;
using ForgeFit.Domain.Constants;

namespace ForgeFit.Api.Validations.Workout;

public class WorkoutProgramRequestValidator : AbstractValidator<WorkoutProgramRequest>
{
    public WorkoutProgramRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(DomainConstants.ValidationLimits.MaxWorkoutProgramNameLength).WithMessage($"Name must be less than {DomainConstants.ValidationLimits.MaxWorkoutProgramNameLength} characters long.");

        RuleFor(x => x.Description)
            .MaximumLength(DomainConstants.ValidationLimits.MaxWorkoutProgramDescriptionLength).WithMessage($"Description must be less than {DomainConstants.ValidationLimits.MaxWorkoutProgramDescriptionLength} characters long.");

        RuleFor(x => x.WorkoutExercisePlans)
            .NotNull()
            .Must(x => x.Count <= DomainConstants.ValidationLimits.MaxExercisesPerProgram).WithMessage($"Program cannot have more than {DomainConstants.ValidationLimits.MaxExercisesPerProgram} exercises.");

        RuleForEach(x => x.WorkoutExercisePlans).SetValidator(new WorkoutExercisePlanDtoValidator());
    }
}
