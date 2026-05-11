using FluentValidation;
using ForgeFit.Application.DTOs.Workout;
using ForgeFit.Domain.Constants;

namespace ForgeFit.Api.Validations.Workout;

public class WorkoutExercisePlanDtoValidator : AbstractValidator<WorkoutExercisePlanDto>
{
    public WorkoutExercisePlanDtoValidator()
    {
        RuleFor(x => x.WorkoutExercise)
            .NotNull();

        RuleFor(x => x.WorkoutSets)
            .NotNull()
            .Must(x => x.Count <= DomainConstants.ValidationLimits.MaxSetsPerExercise).WithMessage($"Too many sets for one exercise (max {DomainConstants.ValidationLimits.MaxSetsPerExercise}).");

        RuleForEach(x => x.WorkoutSets).SetValidator(new WorkoutSetDtoValidator());
    }
}
