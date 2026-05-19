using FluentValidation;
using ForgeFit.Application.DTOs.Food;
using ForgeFit.Domain.Constants;

namespace ForgeFit.Api.Validations.Food;

public class FoodEntryCreateRequestValidator : AbstractValidator<FoodEntryCreateRequest>
{
    public FoodEntryCreateRequestValidator()
    {
        RuleFor(x => x.DayTime).IsInEnum();

        RuleFor(x => x.FoodItems)
            .NotEmpty().WithMessage("At least one food item is required.")
            .Must(items => items.Count <= DomainConstants.ValidationLimits.MaxFoodItemsPerMeal).WithMessage($"Maximum {DomainConstants.ValidationLimits.MaxFoodItemsPerMeal} items per meal allowed.");

        RuleForEach(x => x.FoodItems).SetValidator(new FoodItemDtoValidator());
    }
}
