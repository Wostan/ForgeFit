using FluentValidation;
using ForgeFit.Application.DTOs.Goal;

namespace ForgeFit.Api.Validations.Goal;

public class NutritionGoalCreateRequestValidator : AbstractValidator<NutritionGoalCreateRequest>
{
    public NutritionGoalCreateRequestValidator()
    {
        RuleFor(x => x.Calories)
            .InclusiveBetween(500, 10000).WithMessage("Daily calories must be between 500 and 10000.");

        RuleFor(x => x.Carbs)
            .GreaterThan(0).WithMessage("Carbs must be greater than 0.");

        RuleFor(x => x.Protein)
            .GreaterThan(0).WithMessage("Protein must be greater than 0.");

        RuleFor(x => x.Fat)
            .GreaterThan(0).WithMessage("Fat must be greater than 0.");

        RuleFor(x => x.WaterGoalMl)
            .InclusiveBetween(1000, 10000).WithMessage("Water goal should be at least 1000ml.");
    }
}
