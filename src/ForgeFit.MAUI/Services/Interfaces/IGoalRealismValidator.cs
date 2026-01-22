using ForgeFit.MAUI.Models.Enums.GoalEnums;
using ForgeFit.MAUI.Models.Enums.ProfileEnums;

namespace ForgeFit.MAUI.Services.Interfaces;

public interface IGoalRealismValidator
{
    (bool IsValid, string ErrorMessage) ValidateGoalRealism(
        double currentWeight,
        double targetWeight,
        double heightCm,
        DateTime? dueDate,
        GoalType type,
        WeightUnit unit);
}
