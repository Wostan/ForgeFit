using FluentValidation;
using ForgeFit.Application.DTOs.Food;
using ForgeFit.Domain.Constants;

namespace ForgeFit.Api.Validations.Food;

public class DrinkEntryCreateRequestValidator : AbstractValidator<DrinkEntryCreateRequest>
{
    public DrinkEntryCreateRequestValidator()
    {
        RuleFor(x => x.VolumeMl)
            .InclusiveBetween(DomainConstants.ValidationLimits.MinDrinkVolumeMl, DomainConstants.ValidationLimits.MaxDrinkVolumeMl).WithMessage($"Drink volume must be between {DomainConstants.ValidationLimits.MinDrinkVolumeMl}ml and {DomainConstants.ValidationLimits.MaxDrinkVolumeMl}ml.");
    }
}
