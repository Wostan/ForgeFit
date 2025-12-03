using ForgeFit.MAUI.Models.Enums.GoalEnums;
using ForgeFit.MAUI.Models.Enums.ProfileEnums;

namespace ForgeFit.MAUI.Models.DTOs.Goal;

public record BodyGoalCreateRequest(
    string Title,
    string? Description,
    double WeightGoal,
    WeightUnit WeightUnit,
    DateTime? DueDate,
    GoalType GoalType);
