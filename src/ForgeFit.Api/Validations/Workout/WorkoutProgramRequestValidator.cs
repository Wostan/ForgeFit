using FluentValidation;
using ForgeFit.Application.DTOs.Workout;

namespace ForgeFit.Api.Validations.Workout;

public class WorkoutProgramRequestValidator : AbstractValidator<WorkoutProgramRequest>
{
    public WorkoutProgramRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(50).WithMessage("Name must be less than 50 characters long.");
        
        RuleFor(x => x.Description)
            .MaximumLength(300).WithMessage("Description must be less than 300 characters long.");
        
        RuleFor(x => x.WorkoutExercisePlans)
            .NotNull()
            .Must(x => x.Count > 0).WithMessage("Program must contain at least one exercise.")
            .Must(x => x.Count <= 50).WithMessage("Program cannot have more than 50 exercises.");
        
        RuleForEach(x => x.WorkoutExercisePlans).SetValidator(new WorkoutExercisePlanDtoValidator());
    }
}