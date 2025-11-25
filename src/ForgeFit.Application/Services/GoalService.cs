using ForgeFit.Application.Common.Exceptions;
using ForgeFit.Application.Common.Interfaces.Repositories;
using ForgeFit.Application.Common.Interfaces.Services;
using ForgeFit.Application.Common.Interfaces.Services.InfrastructureServices;
using ForgeFit.Application.DTOs.Goal;
using ForgeFit.Domain.ValueObjects;
using MapsterMapper;

namespace ForgeFit.Application.Services;

public class GoalService(
    IBodyGoalRepository bodyGoalRepository,
    INutritionGoalRepository nutritionGoalRepository,
    IWorkoutGoalRepository workoutGoalRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper) : IGoalService
{
    public async Task<BodyGoalResponse> GetBodyGoalAsync(Guid userId)
    {
        var bodyGoal = await bodyGoalRepository.GetByUserIdAsync(userId);
        
        return bodyGoal is null 
            ? throw new NotFoundException("Body goal not found") 
            : mapper.Map<BodyGoalResponse>(bodyGoal);
    }

    public async Task<NutritionGoalResponse> GetNutritionGoalAsync(Guid userId)
    {
        var nutritionGoal = await nutritionGoalRepository.GetByUserIdAsync(userId);
        
        return nutritionGoal is null 
            ? throw new NotFoundException("Nutrition goal not found") 
            : mapper.Map<NutritionGoalResponse>(nutritionGoal);
    }

    public async Task<WorkoutGoalResponse> GetWorkoutGoalAsync(Guid userId)
    {
        var workoutGoal = await workoutGoalRepository.GetByUserIdAsync(userId);
        
        return workoutGoal is null 
            ? throw new NotFoundException("Workout goal not found") 
            : mapper.Map<WorkoutGoalResponse>(workoutGoal);
    }

    public async Task<BodyGoalResponse> UpdateBodyGoalAsync(Guid userId, BodyGoalCreateRequest bodyGoalRequest)
    {
        var bodyGoal = await bodyGoalRepository.GetByUserIdAsync(userId);
        if (bodyGoal is null)
            throw new NotFoundException("Body goal not found");
        
        bodyGoal.UpdateInfo(
            bodyGoalRequest.Title, 
            bodyGoalRequest.Description, 
            bodyGoalRequest.DueDate);
        bodyGoal.UpdateGoalType(bodyGoalRequest.GoalType);
        
        var newWeightGoal = new Weight(bodyGoalRequest.WeightGoal, bodyGoalRequest.WeightUnit);
        bodyGoal.UpdateWeightGoal(newWeightGoal);
        
        await unitOfWork.SaveChangesAsync();
        
        return mapper.Map<BodyGoalResponse>(bodyGoal);
    }

    public async Task<NutritionGoalResponse> UpdateNutritionGoalAsync(
        Guid userId, 
        NutritionGoalCreateRequest nutritionGoalRequest)
    {
        var nutritionGoal = await nutritionGoalRepository.GetByUserIdAsync(userId);
        if (nutritionGoal is null)
            throw new NotFoundException("Nutrition goal not found");
        
        nutritionGoal.UpdateNutritionGoal(
            nutritionGoalRequest.Calories, 
            nutritionGoalRequest.Carbs,
            nutritionGoalRequest.Protein, 
            nutritionGoalRequest.Fat);
        nutritionGoal.UpdateWaterGoalMl(nutritionGoalRequest.WaterGoalMl);
        
        await unitOfWork.SaveChangesAsync();
        
        return mapper.Map<NutritionGoalResponse>(nutritionGoal);
    }

    public async Task<WorkoutGoalResponse> UpdateWorkoutGoalAsync(
        Guid userId,
        WorkoutGoalCreateRequest workoutGoalRequest)
    {
        var workoutGoal = await workoutGoalRepository.GetByUserIdAsync(userId);
        if (workoutGoal is null)
            throw new NotFoundException("Workout goal not found");
        
        workoutGoal.UpdateWorkoutGoal(
            workoutGoalRequest.WorkoutsPerWeek,
            workoutGoalRequest.Duration,
            workoutGoalRequest.WorkoutType);
        
        await unitOfWork.SaveChangesAsync();
        
        return mapper.Map<WorkoutGoalResponse>(workoutGoal);
    }
}