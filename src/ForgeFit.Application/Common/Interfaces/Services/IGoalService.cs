using ForgeFit.Application.DTOs.Goal;

namespace ForgeFit.Application.Common.Interfaces.Services;

public interface IGoalService
{
    Task<BodyGoalResponse> GetBodyGoalAsync(Guid userId);
    Task<NutritionGoalResponse> GetNutritionGoalAsync(Guid userId);
    Task<WorkoutGoalResponse> GetWorkoutGoalAsync(Guid userId);
    
    Task<BodyGoalResponse> UpdateBodyGoalAsync(Guid userId, BodyGoalCreateRequest bodyGoal);
    Task<NutritionGoalResponse> UpdateNutritionGoalAsync(Guid userId, NutritionGoalCreateRequest nutritionGoal);
    Task<WorkoutGoalResponse> UpdateWorkoutGoalAsync(Guid userId, WorkoutGoalCreateRequest workoutGoal);
}