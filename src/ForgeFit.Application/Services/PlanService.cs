using ForgeFit.Application.Common.Exceptions;
using ForgeFit.Application.Common.Interfaces.Repositories;
using ForgeFit.Application.Common.Interfaces.Services;
using ForgeFit.Application.Common.Interfaces.Services.InfrastructureServices;
using ForgeFit.Application.DTOs.Goal;
using ForgeFit.Application.DTOs.Plan;
using ForgeFit.Domain.Aggregates.GoalAggregate;
using ForgeFit.Domain.Enums.GoalEnums;
using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives.Interfaces;
using ForgeFit.Domain.ValueObjects;
using ForgeFit.Domain.ValueObjects.GoalValueObjects;
using MapsterMapper;

namespace ForgeFit.Application.Services;

public class PlanService(
    IUserRepository userRepository,
    IBodyGoalRepository bodyGoalRepository,
    INutritionGoalRepository nutritionGoalRepository,
    IWorkoutGoalRepository workoutGoalRepository,
    IUnitOfWork unitOfWork,
    IPlanGenerationService planGenerationService,
    IMapper mapper
    ) : IPlanService
{
    public async Task<PlanDto> GeneratePlanAsync(Guid userId)
    {
        var user = await userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new NotFoundException("User not found.");
        
        var userProfile = user.UserProfile;
        
        var bodyGoal = await bodyGoalRepository.GetByUserIdAsync(userId);
        if (bodyGoal == null)
            throw new NotFoundException("Body goal not found.");
        
        var (workoutPlan, nutritionPlan) = 
            planGenerationService.GenerateFullPlan(userProfile, bodyGoal);

        var nutritionDto = new NutritionGoalResponse(
            Guid.Empty,
            nutritionPlan.TargetCalories,
            nutritionPlan.Carbs,
            nutritionPlan.Protein,
            nutritionPlan.Fat,
            nutritionPlan.WaterMl
        );

        var workoutDto = new WorkoutGoalResponse(
            Guid.Empty,
            workoutPlan.WorkoutsPerWeek,
            workoutPlan.Duration,
            workoutPlan.WorkoutType
        );

        var bodyGoalDto = mapper.Map<BodyGoalResponse>(bodyGoal);

        return new PlanDto(bodyGoalDto, nutritionDto, workoutDto);
    }

    public async Task<PlanDto> ConfirmPlanAsync(Guid userId, PlanDto plan)
    {
        var existingBodyGoal = await bodyGoalRepository.GetByUserIdAsync(userId);
        if (existingBodyGoal != null) 
        {
            existingBodyGoal.UpdateInfo(plan.BodyGoal.Title, plan.BodyGoal.Description, plan.BodyGoal.DueDate);
            existingBodyGoal.UpdateGoalType(plan.BodyGoal.GoalType);
            existingBodyGoal.UpdateWeightGoal(new Weight(plan.BodyGoal.WeightGoal, plan.BodyGoal.WeightUnit));
        }
        else 
        {
             var newBody = BodyGoal.Create(
                userId,
                plan.BodyGoal.Title,
                plan.BodyGoal.Description,
                new Weight(plan.BodyGoal.WeightGoal, plan.BodyGoal.WeightUnit),
                plan.BodyGoal.DueDate,
                plan.BodyGoal.GoalType,
                GoalStatus.InProgress);
             await bodyGoalRepository.AddAsync(newBody);
        }

        var existingNutritionGoal = await nutritionGoalRepository.GetByUserIdAsync(userId);
        if (existingNutritionGoal != null)
        {
             existingNutritionGoal.UpdateNutritionGoal(
                plan.NutritionGoal.Calories,
                plan.NutritionGoal.Carbs,
                plan.NutritionGoal.Protein,
                plan.NutritionGoal.Fat,
                plan.NutritionGoal.WaterGoalMl);
        }
        else
        {
            var nutritionGoal = NutritionGoal.Create(
                userId,
                new DailyNutritionPlan(
                    plan.NutritionGoal.Calories,
                    plan.NutritionGoal.Carbs,
                    plan.NutritionGoal.Protein,
                    plan.NutritionGoal.Fat,
                    plan.NutritionGoal.WaterGoalMl));
            await nutritionGoalRepository.AddAsync(nutritionGoal);
        }
        
        var existingWorkoutGoal = await workoutGoalRepository.GetByUserIdAsync(userId);
        if (existingWorkoutGoal != null)
        {
            existingWorkoutGoal.UpdateWorkoutGoal(
                plan.WorkoutGoal.WorkoutsPerWeek,
                plan.WorkoutGoal.Duration,
                plan.WorkoutGoal.WorkoutType);
        }
        else
        {
             var workoutGoal = WorkoutGoal.Create(
                userId,
                new WorkoutPlan(
                    plan.WorkoutGoal.WorkoutsPerWeek,
                    plan.WorkoutGoal.Duration,
                    plan.WorkoutGoal.WorkoutType));
             await workoutGoalRepository.AddAsync(workoutGoal);
        }
        
        await unitOfWork.SaveChangesAsync();
        
        return plan;
    }

    public async Task<PlanDto> GetPlanAsync(Guid userId)
    {
        var bodyGoal = await bodyGoalRepository.GetByUserIdAsync(userId);
        var nutritionGoal = await nutritionGoalRepository.GetByUserIdAsync(userId);
        var workoutGoal = await workoutGoalRepository.GetByUserIdAsync(userId);
        
        if (bodyGoal == null || nutritionGoal == null || workoutGoal == null)
            throw new DomainValidationException("Plan not found.");
        
        var planDto = new PlanDto(
            mapper.Map<BodyGoalResponse>(bodyGoal),
            mapper.Map<NutritionGoalResponse>(nutritionGoal),
            mapper.Map<WorkoutGoalResponse>(workoutGoal)
        );
        
        return planDto;
    }
}