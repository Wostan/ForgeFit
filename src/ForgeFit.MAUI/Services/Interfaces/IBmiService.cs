using ForgeFit.MAUI.Models.Enums.GoalEnums;

namespace ForgeFit.MAUI.Services.Interfaces;

public interface IBmiService
{
    double CalculateBmi(double weight, double heightCm); 
    GoalType DetermineGoalType(double currentWeight, double targetWeight, double heightCm);
}
