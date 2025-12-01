using FluentValidation;
using ForgeFit.Application.DTOs.Goal;
using ForgeFit.Domain.Enums.ProfileEnums;

namespace ForgeFit.Api.Validations.Goal;

public class BodyGoalCreateRequestValidator : AbstractValidator<BodyGoalCreateRequest>
{
    public BodyGoalCreateRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(20).WithMessage("Title must not exceed 20 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(200).WithMessage("Description must not exceed 200 characters.");

        RuleFor(x => x.WeightUnit).IsInEnum();
        RuleFor(x => x.WeightGoal)
            .GreaterThan(0)
            .InclusiveBetween(30, 300).When(x => x.WeightUnit == WeightUnit.Kg)
            .InclusiveBetween(66, 660).When(x => x.WeightUnit == WeightUnit.Lb)
            .WithMessage("Target weight must be realistic.");


        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow).When(x => x.DueDate.HasValue)
            .WithMessage("Due date must be in the future.");

        RuleFor(x => x.GoalType).IsInEnum();
    }
}
