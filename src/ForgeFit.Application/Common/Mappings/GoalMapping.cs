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
            .Map(dest => dest.Id, src => src.Id);

        config.NewConfig<WorkoutGoal, WorkoutGoalResponse>()
            .Map(dest => dest.Id, src => src.Id);
    }
}