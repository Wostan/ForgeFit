using FluentValidation;
using ForgeFit.Api.Validations.Goal;
using ForgeFit.Api.Validations.User;
using ForgeFit.Application.DTOs.Plan;

namespace ForgeFit.Api.Validations.Plan;

public class GeneratePlanRequestValidator : AbstractValidator<GeneratePlanRequest>
{
    public GeneratePlanRequestValidator()
    {
        RuleFor(x => x.UserProfile)
            .NotNull()
            .SetValidator(new UserProfileDtoValidator());
        
        RuleFor(x => x.BodyGoal)
            .NotNull()
            .SetValidator(new BodyGoalCreateRequestValidator());
    }
}