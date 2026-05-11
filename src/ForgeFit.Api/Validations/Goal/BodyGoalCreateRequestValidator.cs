using FluentValidation;
using ForgeFit.Application.DTOs.Goal;
using ForgeFit.Domain.Constants;
using ForgeFit.Domain.Enums.ProfileEnums;

namespace ForgeFit.Api.Validations.Goal;

public class BodyGoalCreateRequestValidator : AbstractValidator<BodyGoalCreateRequest>
{
    public BodyGoalCreateRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(DomainConstants.ValidationLimits.MaxTitleLength).WithMessage($"Title must not exceed {DomainConstants.ValidationLimits.MaxTitleLength} characters.");

        RuleFor(x => x.Description)
            .MaximumLength(DomainConstants.ValidationLimits.MaxDescriptionLength).WithMessage($"Description must not exceed {DomainConstants.ValidationLimits.MaxDescriptionLength} characters.");

        RuleFor(x => x.WeightUnit).IsInEnum();
        RuleFor(x => x.WeightGoal)
            .GreaterThan(0)
            .InclusiveBetween(DomainConstants.ValidationLimits.MinWeightKg, DomainConstants.ValidationLimits.MaxWeightKg).When(x => x.WeightUnit == WeightUnit.Kg)
            .InclusiveBetween(DomainConstants.ValidationLimits.MinWeightLbs, DomainConstants.ValidationLimits.MaxWeightLbs).When(x => x.WeightUnit == WeightUnit.Lb)
            .WithMessage("Target weight must be realistic.");


        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow).When(x => x.DueDate.HasValue)
            .WithMessage("Due date must be in the future.");

        RuleFor(x => x.GoalType).IsInEnum();
    }
}
