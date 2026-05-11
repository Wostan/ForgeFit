using ForgeFit.MAUI.Constants;
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
        var currentKg = unit == WeightUnit.Kg ? currentWeight : currentWeight * AppConstants.ConversionFactors.LbsToKg;
        var targetKg = unit == WeightUnit.Kg ? targetWeight : targetWeight * AppConstants.ConversionFactors.LbsToKg;

        var targetBmi = bmiService.CalculateBmi(targetKg, heightCm);

        switch (type)
        {
            case GoalType.FatLoss when targetBmi is > 0 and < AppConstants.BmiThresholds.UnderweightMax:
                return (false, localizationManager["Error_TargetWeightTooLow"]);
            case GoalType.MuscleGain:
            case GoalType.WeightGain:
            {
                if (targetBmi > AppConstants.BmiThresholds.OverweightMax)
                    return (false, localizationManager["Error_TargetWeightTooHigh"]);
                break;
            }
        }

        if (!dueDate.HasValue)
            return (true, string.Empty);

        var days = (dueDate.Value - DateTime.UtcNow).TotalDays;

        if (days < AppConstants.GoalValidation.MinDaysToDeadline)
            return (false, localizationManager["Error_DeadlineTooClose"]);

        var weeks = days / 7.0;

        var ratePerWeek = Math.Abs(targetKg - currentKg) / weeks;

        switch (type)
        {
            case GoalType.FatLoss when ratePerWeek > AppConstants.GoalValidation.MaxFatLossKgPerWeek:
                var msgFat = string.Format(localizationManager["Error_FatLossUnsafe"], ratePerWeek.ToString("F1"));
                return (false, msgFat);

            case GoalType.MuscleGain when ratePerWeek > AppConstants.GoalValidation.MaxMuscleGainKgPerWeek:
                var msgMuscle = string.Format(localizationManager["Error_MuscleGainUnrealistic"],
                    ratePerWeek.ToString("F1"));
                return (false, msgMuscle);

            case GoalType.WeightGain when ratePerWeek > AppConstants.GoalValidation.MaxWeightGainKgPerWeek:
                var msgGain = string.Format(localizationManager["Error_WeightGainUnrealistic"],
                    ratePerWeek.ToString("F1"));
                return (false, msgGain);

            default:
                return (true, string.Empty);
        }
    }
}
