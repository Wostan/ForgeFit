using ForgeFit.Application.DTOs.Goal;
using ForgeFit.Application.DTOs.User;

namespace ForgeFit.Application.DTOs.Plan;

public record GeneratePlanRequest(
    UserProfileDto UserProfile,
    BodyGoalCreateRequest BodyGoal);