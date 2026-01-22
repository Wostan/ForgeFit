using ForgeFit.Application.DTOs.Goal;
using ForgeFit.Domain.Aggregates.GoalAggregate;
using Mapster;

namespace ForgeFit.Application.Common.Mappings;

public class GoalMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<BodyGoal, BodyGoalResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.WeightGoal, src => src.WeightGoal.Value)
            .Map(dest => dest.WeightUnit, src => src.WeightGoal.Unit);

        config.NewConfig<NutritionGoal, NutritionGoalResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Calories, src => src.DailyNutritionPlan.TargetCalories)
            .Map(dest => dest.Carbs, src => src.DailyNutritionPlan.Carbs)
            .Map(dest => dest.Protein, src => src.DailyNutritionPlan.Protein)
            .Map(dest => dest.Fat, src => src.DailyNutritionPlan.Fat)
            .Map(dest => dest.WaterGoalMl, src => src.DailyNutritionPlan.WaterMl);

        config.NewConfig<WorkoutGoal, WorkoutGoalResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.WorkoutsPerWeek, src => src.WorkoutPlan.WorkoutsPerWeek)
            .Map(dest => dest.Duration, src => src.WorkoutPlan.Duration)
            .Map(dest => dest.WorkoutType, src => src.WorkoutPlan.WorkoutType);
    }
}
