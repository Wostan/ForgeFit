using ForgeFit.Application.Common.Exceptions;
using ForgeFit.Application.Common.Interfaces.Repositories;
using ForgeFit.Application.Common.Interfaces.Services;
using ForgeFit.Application.Common.Interfaces.Services.InfrastructureServices;
using ForgeFit.Application.DTOs.Goal;
using ForgeFit.Domain.Aggregates.GoalAggregate;
using ForgeFit.Domain.Enums.GoalEnums;
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

        if (bodyGoal == null)
        {
            throw new NotFoundException("Body goal not found");
        }
        
        if (userId != bodyGoal.UserId)
        {
            throw new UnauthorizedAccessException("You do not own this body goal");
        }
        
        return mapper.Map<BodyGoalResponse>(bodyGoal);
    }

    public async Task<NutritionGoalResponse> GetNutritionGoalAsync(Guid userId)
    {
        var nutritionGoal = await nutritionGoalRepository.GetByUserIdAsync(userId);

        if (nutritionGoal is null)
        {
            throw new NotFoundException("Nutrition goal not found");
        }
        
        if (userId != nutritionGoal.UserId)
        {
            throw new UnauthorizedAccessException("You do not own this nutrition goal");
        }

        return mapper.Map<NutritionGoalResponse>(nutritionGoal);
    }

    public async Task<WorkoutGoalResponse> GetWorkoutGoalAsync(Guid userId)
    {
        var workoutGoal = await workoutGoalRepository.GetByUserIdAsync(userId);

        if (workoutGoal is null)
        {
            throw new NotFoundException("Workout goal not found");
        }
        
        if (userId != workoutGoal.UserId)
        {
            throw new UnauthorizedAccessException("You do not own this workout goal");
        }
        
        return mapper.Map<WorkoutGoalResponse>(workoutGoal);
    }
    
    public async Task<BodyGoalResponse> CreateBodyGoalAsync(Guid userId, BodyGoalCreateRequest bodyGoalRequest)
    {
        var bodyGoal = BodyGoal.Create(
            userId,
            bodyGoalRequest.Title,
            bodyGoalRequest.Description,
            new Weight(bodyGoalRequest.WeightGoal, bodyGoalRequest.WeightUnit),
            bodyGoalRequest.DueDate,
            bodyGoalRequest.GoalType,
            GoalStatus.InProgress);
        
        await bodyGoalRepository.AddAsync(bodyGoal);
        await unitOfWork.SaveChangesAsync();
        
        return mapper.Map<BodyGoalResponse>(bodyGoal);
    }

    public async Task<BodyGoalResponse> UpdateBodyGoalAsync(Guid userId, BodyGoalCreateRequest bodyGoalRequest)
    {
        var bodyGoal = await bodyGoalRepository.GetByUserIdAsync(userId);
        if (bodyGoal is null)
        {
            throw new NotFoundException("Body goal not found");
        }
        
        if (userId != bodyGoal.UserId)
        {
            throw new UnauthorizedAccessException("You do not own this body goal");
        }
        
        bodyGoal.Update(
            bodyGoalRequest.Title,
            bodyGoalRequest.Description,
            bodyGoalRequest.DueDate,
            new Weight(bodyGoalRequest.WeightGoal, bodyGoalRequest.WeightUnit),
            bodyGoalRequest.GoalType);
        
        await unitOfWork.SaveChangesAsync();
        
        return mapper.Map<BodyGoalResponse>(bodyGoal);
    }

    public async Task<NutritionGoalResponse> UpdateNutritionGoalAsync(
        Guid userId, 
        NutritionGoalCreateRequest nutritionGoalRequest)
    {
        var nutritionGoal = await nutritionGoalRepository.GetByUserIdAsync(userId);
        if (nutritionGoal is null)
        {
            throw new NotFoundException("Nutrition goal not found");
        }
        
        if (userId != nutritionGoal.UserId)
        {
            throw new UnauthorizedAccessException("You do not own this nutrition goal");
        }
        
        nutritionGoal.Update(
            nutritionGoalRequest.Calories,
            nutritionGoalRequest.Carbs,
            nutritionGoalRequest.Protein,
            nutritionGoalRequest.Fat,
            nutritionGoalRequest.WaterGoalMl);
        
        await unitOfWork.SaveChangesAsync();
        
        return mapper.Map<NutritionGoalResponse>(nutritionGoal);
    }

    public async Task<WorkoutGoalResponse> UpdateWorkoutGoalAsync(
        Guid userId,
        WorkoutGoalCreateRequest workoutGoalRequest)
    {
        var workoutGoal = await workoutGoalRepository.GetByUserIdAsync(userId);
        if (workoutGoal is null)
        {
            throw new NotFoundException("Workout goal not found");
        }
        
        if (userId != workoutGoal.UserId)
        {
            throw new UnauthorizedAccessException("You do not own this workout goal");
        }
        
        workoutGoal.Update(
            workoutGoalRequest.WorkoutsPerWeek,
            workoutGoalRequest.Duration,
            workoutGoalRequest.WorkoutType);
        
        await unitOfWork.SaveChangesAsync();
        
        return mapper.Map<WorkoutGoalResponse>(workoutGoal);
    }
}