using ForgeFit.Domain.Enums.GoalEnums;
using ForgeFit.Domain.Enums.ProfileEnums;

namespace ForgeFit.Application.DTOs.Plan;

public record BodyGoalDto(
    string Title,
    string? Description,
    double WeightGoal,
    WeightUnit WeightUnit,
    DateTime? DueDate,
    GoalType GoalType);