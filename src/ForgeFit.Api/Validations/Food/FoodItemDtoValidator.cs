using FluentValidation;
using ForgeFit.Application.DTOs.Food;

namespace ForgeFit.Api.Validations.Food;

public class FoodItemDtoValidator : AbstractValidator<FoodItemDto>
{
    public FoodItemDtoValidator()
    {
        RuleFor(x => x.ExternalId).NotEmpty();
        
        RuleFor(x => x.Label)
            .NotEmpty()
            .MaximumLength(100).WithMessage("Label must not exceed 100 characters.");
        
        RuleFor(x => x.ServingUnit).NotEmpty();

        RuleFor(x => x.Amount)
            .InclusiveBetween(1, 5000).WithMessage("Amount must be between 1 and 5000.");

        RuleFor(x => x.Calories)
            .GreaterThanOrEqualTo(0);
        RuleFor(x => x.Protein)
            .GreaterThanOrEqualTo(0);
        RuleFor(x => x.Fat)
            .GreaterThanOrEqualTo(0);
        RuleFor(x => x.Carbs)
            .GreaterThanOrEqualTo(0);
    }
}