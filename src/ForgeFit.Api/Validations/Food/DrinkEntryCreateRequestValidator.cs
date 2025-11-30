using FluentValidation;
using ForgeFit.Application.DTOs.Food;

namespace ForgeFit.Api.Validations.Food;

public class DrinkEntryCreateRequestValidator : AbstractValidator<DrinkEntryCreateRequest>
{
    public DrinkEntryCreateRequestValidator()
    {
        RuleFor(x => x.VolumeMl)
            .InclusiveBetween(50, 2000).WithMessage("Drink volume must be between 50ml and 2000ml.");

        RuleFor(x => x.Date)
            .NotEmpty()
            .LessThanOrEqualTo(DateTime.UtcNow.AddHours(24).Date).WithMessage("Cannot log drinks for the day after tomorrow or later.");
    }
}