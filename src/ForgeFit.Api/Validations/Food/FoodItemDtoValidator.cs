using FluentValidation;
using ForgeFit.Application.DTOs.Food;
using ForgeFit.Domain.Constants;

namespace ForgeFit.Api.Validations.Food;

public class FoodItemDtoValidator : AbstractValidator<FoodItemDto>
{
    public FoodItemDtoValidator()
    {
        RuleFor(x => x.ExternalId)
            .NotEmpty()
            .MaximumLength(DomainConstants.ValidationLimits.MaxExternalIdLength).WithMessage($"ExternalId must not exceed {DomainConstants.ValidationLimits.MaxExternalIdLength} characters.");

        RuleFor(x => x.Label)
            .NotEmpty()
            .MaximumLength(DomainConstants.ValidationLimits.MaxFoodLabelLength).WithMessage($"Label must not exceed {DomainConstants.ValidationLimits.MaxFoodLabelLength} characters.");

        RuleFor(x => x.ServingUnit).NotEmpty();

        RuleFor(x => x.Amount)
            .InclusiveBetween(DomainConstants.ValidationLimits.MinFoodAmount, DomainConstants.ValidationLimits.MaxFoodAmount).WithMessage($"Amount must be between {DomainConstants.ValidationLimits.MinFoodAmount} and {DomainConstants.ValidationLimits.MaxFoodAmount}.");

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
