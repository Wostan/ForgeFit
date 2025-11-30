using ForgeFit.Domain.Enums.GoalEnums;
using ForgeFit.Domain.Enums.ProfileEnums;

namespace ForgeFit.Application.DTOs.Goal;

public record BodyGoalResponse(
    Guid Id,
    string Title,
    string? Description,
    double WeightGoal,
    WeightUnit WeightUnit,
    DateTime? DueDate,
    GoalType GoalType,
    GoalStatus GoalStatus);