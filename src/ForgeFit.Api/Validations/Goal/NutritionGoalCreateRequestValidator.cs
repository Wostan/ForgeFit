using FluentValidation;
using ForgeFit.Application.DTOs.Goal;
using ForgeFit.Domain.Constants;

namespace ForgeFit.Api.Validations.Goal;

public class NutritionGoalCreateRequestValidator : AbstractValidator<NutritionGoalCreateRequest>
{
    public NutritionGoalCreateRequestValidator()
    {
        RuleFor(x => x.Calories)
            .InclusiveBetween(DomainConstants.ValidationLimits.MinDailyCalories, DomainConstants.ValidationLimits.MaxDailyCalories).WithMessage($"Daily calories must be between {DomainConstants.ValidationLimits.MinDailyCalories} and {DomainConstants.ValidationLimits.MaxDailyCalories}.");

        RuleFor(x => x.Carbs)
            .GreaterThan(0).WithMessage("Carbs must be greater than 0.");

        RuleFor(x => x.Protein)
            .GreaterThan(0).WithMessage("Protein must be greater than 0.");

        RuleFor(x => x.Fat)
            .GreaterThan(0).WithMessage("Fat must be greater than 0.");

        RuleFor(x => x.WaterGoalMl)
            .InclusiveBetween(DomainConstants.ValidationLimits.MinWaterIntakeMl, DomainConstants.ValidationLimits.MaxWaterIntakeMl).WithMessage($"Water goal should be at least {DomainConstants.ValidationLimits.MinWaterIntakeMl}ml.");
    }
}
