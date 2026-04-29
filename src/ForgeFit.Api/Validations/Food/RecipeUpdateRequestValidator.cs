using FluentValidation;
using ForgeFit.Application.DTOs.Food;
using ForgeFit.Domain.Constants;

namespace ForgeFit.Api.Validations.Food;

public class RecipeUpdateRequestValidator : AbstractValidator<RecipeUpdateRequest>
{
    public RecipeUpdateRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(DomainConstants.ValidationLimits.MaxFoodLabelLength).WithMessage($"Name must not exceed {DomainConstants.ValidationLimits.MaxFoodLabelLength} characters.");

        RuleFor(x => x.Description)
            .MaximumLength(DomainConstants.ValidationLimits.MaxDescriptionLength).WithMessage($"Description must not exceed {DomainConstants.ValidationLimits.MaxDescriptionLength} characters.");

        RuleFor(x => x.Ingredients)
            .NotEmpty().WithMessage("At least one ingredient is required.")
            .Must(items => items.Count <= DomainConstants.ValidationLimits.MaxFoodItemsPerMeal).WithMessage($"Maximum {DomainConstants.ValidationLimits.MaxFoodItemsPerMeal} ingredients per recipe allowed.");

        RuleForEach(x => x.Ingredients).SetValidator(new FoodItemDtoValidator());
    }
}
