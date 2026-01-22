using ForgeFit.MAUI.Models.Enums.GoalEnums;
using ForgeFit.MAUI.Services.Interfaces;

namespace ForgeFit.MAUI.Services;

public class BmiService : IBmiService
{
    public double CalculateBmi(double weight, double heightCm)
    {
        if (heightCm <= 0) return 0;
        var heightM = heightCm / 100.0;
        return weight / (heightM * heightM);
    }

    public GoalType DetermineGoalType(double currentWeight, double targetWeight, double heightCm)
    {
        if (Math.Abs(currentWeight - targetWeight) < 1.0) return GoalType.MuscleGain;

        if (targetWeight < currentWeight) return GoalType.FatLoss;

        var currentBmi = CalculateBmi(currentWeight, heightCm);

        return currentBmi < 18.5 ? GoalType.WeightGain : GoalType.MuscleGain;
    }
}
