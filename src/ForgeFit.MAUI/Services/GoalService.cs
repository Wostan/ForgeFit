using ForgeFit.MAUI.Models;
using ForgeFit.MAUI.Models.DTOs.Goal;
using ForgeFit.MAUI.Services.Interfaces;

namespace ForgeFit.MAUI.Services;

public class GoalService(IApiService apiService) : IGoalService
{
    public async Task<ServiceResponse<BodyGoalResponse?>> GetBodyGoal(CancellationToken cancellationToken = default)
    {
        return await apiService.GetAsync<BodyGoalResponse?>("/api/goal/body", cancellationToken);
    }

    public async Task<ServiceResponse<NutritionGoalResponse?>> GetNutritionGoal(
        CancellationToken cancellationToken = default)
    {
        return await apiService.GetAsync<NutritionGoalResponse?>("/api/goal/nutrition", cancellationToken);
    }

    public async Task<ServiceResponse<WorkoutGoalResponse?>> GetWorkoutGoal(
        CancellationToken cancellationToken = default)
    {
        return await apiService.GetAsync<WorkoutGoalResponse?>("/api/goal/workout", cancellationToken);
    }

    // create body goal
    public async Task<ServiceResponse<BodyGoalResponse?>> CreateBodyGoal(BodyGoalCreateRequest goal,
        CancellationToken cancellationToken = default)
    {
        return await apiService.PostAsync<BodyGoalCreateRequest, BodyGoalResponse?>("/api/goal/body", goal,
            cancellationToken);
    }

    public async Task<ServiceResponse<BodyGoalResponse?>> UpdateBodyGoal(BodyGoalCreateRequest goal,
        CancellationToken cancellationToken = default)
    {
        return await apiService.PutAsync<BodyGoalCreateRequest, BodyGoalResponse>("/api/goal/body", goal,
            cancellationToken);
    }

    public async Task<ServiceResponse<NutritionGoalResponse?>> UpdateNutritionGoal(NutritionGoalCreateRequest goal,
        CancellationToken cancellationToken = default)
    {
        return await apiService.PutAsync<NutritionGoalCreateRequest, NutritionGoalResponse>("/api/goal/nutrition", goal,
            cancellationToken);
    }

    public async Task<ServiceResponse<WorkoutGoalResponse?>> UpdateWorkoutGoal(WorkoutGoalCreateRequest goal,
        CancellationToken cancellationToken = default)
    {
        return await apiService.PutAsync<WorkoutGoalCreateRequest, WorkoutGoalResponse>("/api/goal/workout", goal,
            cancellationToken);
    }
}
