using ForgeFit.Application.DTOs.User;
using ForgeFit.Domain.ValueObjects.UserValueObjects;

namespace ForgeFit.Application.DTOs.Plan;

public record GeneratePlanRequest(
    UserProfileDto UserProfile,
    BodyGoalDto BodyGoal);