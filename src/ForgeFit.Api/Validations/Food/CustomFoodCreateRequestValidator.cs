using FluentValidation;
using ForgeFit.Application.DTOs.Food;
using ForgeFit.Domain.Constants;

namespace ForgeFit.Api.Validations.Food;

public class CustomFoodCreateRequestValidator : AbstractValidator<CustomFoodCreateRequest>
{
    public CustomFoodCreateRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(DomainConstants.ValidationLimits.MaxFoodLabelLength).WithMessage($"Name must not exceed {DomainConstants.ValidationLimits.MaxFoodLabelLength} characters.");

        RuleFor(x => x.Brand)
            .MaximumLength(DomainConstants.ValidationLimits.MaxFoodLabelLength).WithMessage($"Brand must not exceed {DomainConstants.ValidationLimits.MaxFoodLabelLength} characters.");

        RuleFor(x => x.Barcode)
            .MaximumLength(DomainConstants.ValidationLimits.MaxBarcodeLength).WithMessage($"Barcode must not exceed {DomainConstants.ValidationLimits.MaxBarcodeLength} characters.");

        RuleFor(x => x.Calories)
            .GreaterThanOrEqualTo(0);
        RuleFor(x => x.Carbs)
            .GreaterThanOrEqualTo(0);
        RuleFor(x => x.Protein)
            .GreaterThanOrEqualTo(0);
        RuleFor(x => x.Fat)
            .GreaterThanOrEqualTo(0);
        RuleFor(x => x.Fiber)
            .GreaterThanOrEqualTo(0);
        RuleFor(x => x.Sugar)
            .GreaterThanOrEqualTo(0);
        RuleFor(x => x.SaturatedFat)
            .GreaterThanOrEqualTo(0);
        RuleFor(x => x.Sodium)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.ServingSize)
            .GreaterThan(0);

        RuleFor(x => x.ServingUnit)
            .NotEmpty()
            .MaximumLength(DomainConstants.ValidationLimits.MaxServingUnitLength).WithMessage($"Serving unit must not exceed {DomainConstants.ValidationLimits.MaxServingUnitLength} characters.");
    }
}
