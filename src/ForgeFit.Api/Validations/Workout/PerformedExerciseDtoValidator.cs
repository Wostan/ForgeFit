using FluentValidation;
using ForgeFit.Application.DTOs.Workout;

namespace ForgeFit.Api.Validations.Workout;

public class PerformedExerciseDtoValidator : AbstractValidator<PerformedExerciseDto>
{
    public PerformedExerciseDtoValidator()
    {
        RuleFor(x => x.ExerciseSnapshot)
            .NotNull();
        
        RuleFor(x => x.Sets)
            .NotNull()
            .Must(x => x.Count > 0).WithMessage("Performed exercise must have at least one set.")
            .Must(x => x.Count <= 20).WithMessage("Max 20 sets per exercise allowed.");

        RuleForEach(x => x.Sets).SetValidator(new PerformedSetDtoValidator());
    }
}