using FluentValidation;
using ForgeFit.Application.DTOs.Workout;

namespace ForgeFit.Api.Validations.Workout;

public class PerformedSetDtoValidator : AbstractValidator<PerformedSetDto>
{
    public PerformedSetDtoValidator()
    {
        RuleFor(x => x.Order)
            .GreaterThan(0).WithMessage("Order must be greater than 0.");

        RuleFor(x => x.Reps)
            .InclusiveBetween(0, 100).WithMessage("Reps must be between 0 and 100.");
        
        RuleFor(x => x.Weight)
            .GreaterThanOrEqualTo(0)
            .LessThan(1500).WithMessage("Weight seems unreasonably high (>1500).");
        
        RuleFor(x => x.WeightUnit)
            .IsInEnum();
    }
}