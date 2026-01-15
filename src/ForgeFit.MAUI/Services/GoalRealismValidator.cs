using ForgeFit.MAUI.Models.Enums.GoalEnums;
using ForgeFit.MAUI.Models.Enums.ProfileEnums;
using ForgeFit.MAUI.Services.Interfaces;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.Services;

public class GoalRealismValidator(
    ILocalizationResourceManager localizationManager,
    IBmiService bmiService)
    : IGoalRealismValidator
{
    public (bool IsValid, string ErrorMessage) ValidateGoalRealism(
        double currentWeight, 
        double targetWeight, 
        double heightCm,
        DateTime? dueDate,
        GoalType type, 
        WeightUnit unit)
    {
        var currentKg = unit == WeightUnit.Kg ? currentWeight : currentWeight * 0.453592;
        var targetKg = unit == WeightUnit.Kg ? targetWeight : targetWeight * 0.453592;

        var targetBmi = bmiService.CalculateBmi(targetKg, heightCm);

        switch (type)
        {
            case GoalType.FatLoss when targetBmi is > 0 and < 18.5:
                return (false, localizationManager["Error_TargetWeightTooLow"]);
            case GoalType.MuscleGain:
            case GoalType.WeightGain:
            {
                if (targetBmi > 30.0)
                    return (false, localizationManager["Error_TargetWeightTooHigh"]);
                break;
            }
        }

        if (!dueDate.HasValue) 
            return (true, string.Empty);

        var days = (dueDate.Value - DateTime.UtcNow).TotalDays;

        if (days < 7)
        {
            return (false, localizationManager["Error_DeadlineTooClose"]);
        }
        
        var weeks = days / 7.0;
        
        var ratePerWeek = Math.Abs(targetKg - currentKg) / weeks;

        switch (type)
        {
            case GoalType.FatLoss when ratePerWeek > 1.3:
                var msgFat = string.Format(localizationManager["Error_FatLossUnsafe"], ratePerWeek.ToString("F1"));
                return (false, msgFat);

            case GoalType.MuscleGain when ratePerWeek > 0.6:
                var msgMuscle = string.Format(localizationManager["Error_MuscleGainUnrealistic"], ratePerWeek.ToString("F1"));
                return (false, msgMuscle);

            case GoalType.WeightGain when ratePerWeek > 1.5:
                 var msgGain = string.Format(localizationManager["Error_WeightGainUnrealistic"], ratePerWeek.ToString("F1"));
                 return (false, msgGain);

            default:
                return (true, string.Empty);
        }
    }
}
