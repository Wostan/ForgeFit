using ForgeFit.MAUI.Models.DTOs.Goal;
using ForgeFit.MAUI.Models.DTOs.User;

namespace ForgeFit.MAUI.Models.DTOs.Plan;

public record GeneratePlanRequest(
    UserProfileDto UserProfile,
    BodyGoalCreateRequest BodyGoal);
