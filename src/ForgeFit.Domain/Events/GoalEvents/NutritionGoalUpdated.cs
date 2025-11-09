using ForgeFit.Domain.Primitives;

namespace ForgeFit.Domain.Events.GoalEvents;

public class NutritionGoalUpdated(
    Guid goalId,
    int calories,
    int carbs,
    int protein,
    int fat,
    int waterGoalMl) : DomainEvent
{
    public Guid GoalId { get; } = goalId;
    public int Calories { get; } = calories;
    public int Carbs { get; } = carbs;
    public int Protein { get; } = protein;
    public int Fat { get; } = fat;
    public int WaterGoalMl { get; } = waterGoalMl;
}