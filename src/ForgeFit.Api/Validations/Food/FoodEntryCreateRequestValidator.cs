using FluentValidation;
using ForgeFit.Application.DTOs.Food;

namespace ForgeFit.Api.Validations.Food;

public class FoodEntryCreateRequestValidator : AbstractValidator<FoodEntryCreateRequest>
{
    public FoodEntryCreateRequestValidator()
    {
        RuleFor(x => x.DayTime).IsInEnum();

        RuleFor(x => x.Date)
            .NotEmpty()
            .LessThanOrEqualTo(DateTime.UtcNow.AddHours(24).Date)
            .WithMessage("Cannot log food for the day after tomorrow or later.");

        RuleFor(x => x.FoodItems)
            .NotEmpty().WithMessage("At least one food item is required.")
            .Must(items => items.Count <= 20).WithMessage("Maximum 20 items per meal allowed.");

        RuleForEach(x => x.FoodItems).SetValidator(new FoodItemDtoValidator());
    }
}
