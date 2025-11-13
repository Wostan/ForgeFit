using ForgeFit.Application.DTOs.Goal;

namespace ForgeFit.Application.Common.Interfaces.Services;

public interface IGoalService
{
    Task<BodyGoalDto> GetBodyGoalAsync(Guid userId);
    Task<NutritionGoalDto> GetNutritionGoalAsync(Guid userId);
    Task<WorkoutGoalDto> GetWorkoutGoalAsync(Guid userId);
    
    Task<BodyGoalDto> UpdateBodyGoalAsync(Guid userId, BodyGoalDto bodyGoal);
    Task<NutritionGoalDto> UpdateNutritionGoalAsync(Guid userId, NutritionGoalDto nutritionGoal);
    Task<WorkoutGoalDto> UpdateWorkoutGoalAsync(Guid userId, WorkoutGoalDto workoutGoal);
}