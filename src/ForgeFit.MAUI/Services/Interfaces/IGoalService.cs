using ForgeFit.MAUI.Models;
using ForgeFit.MAUI.Models.DTOs.Goal;

namespace ForgeFit.MAUI.Services.Interfaces;

public interface IGoalService
{
    Task<ServiceResponse<BodyGoalResponse?>> GetBodyGoal(CancellationToken cancellationToken = default);
    Task<ServiceResponse<NutritionGoalResponse?>> GetNutritionGoal(CancellationToken cancellationToken = default);
    Task<ServiceResponse<WorkoutGoalResponse?>> GetWorkoutGoal(CancellationToken cancellationToken = default);
    
    // create body goal
    Task<ServiceResponse<BodyGoalResponse?>> CreateBodyGoal(BodyGoalCreateRequest goal, CancellationToken cancellationToken = default);
    
    Task<ServiceResponse<BodyGoalResponse?>> UpdateBodyGoal(BodyGoalCreateRequest goal, CancellationToken cancellationToken = default);
    Task<ServiceResponse<NutritionGoalResponse?>> UpdateNutritionGoal(NutritionGoalCreateRequest goal, CancellationToken cancellationToken = default);
    Task<ServiceResponse<WorkoutGoalResponse?>> UpdateWorkoutGoal(WorkoutGoalCreateRequest goal, CancellationToken cancellationToken = default);
}
