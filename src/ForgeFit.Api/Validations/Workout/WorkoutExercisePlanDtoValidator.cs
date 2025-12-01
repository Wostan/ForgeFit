using FluentValidation;
using ForgeFit.Application.DTOs.Workout;

namespace ForgeFit.Api.Validations.Workout;

public class WorkoutExercisePlanDtoValidator : AbstractValidator<WorkoutExercisePlanDto>
{
    public WorkoutExercisePlanDtoValidator()
    {
        RuleFor(x => x.WorkoutExercise)
            .NotNull();

        RuleFor(x => x.WorkoutSets)
            .NotNull()
            .Must(x => x.Count > 0).WithMessage("Exercise must have at least one set.")
            .Must(x => x.Count <= 20).WithMessage("Too many sets for one exercise (max 20).");

        RuleForEach(x => x.WorkoutSets).SetValidator(new WorkoutSetDtoValidator());
    }
}
