using ForgeFit.Domain.Primitives;

namespace ForgeFit.Domain.Events.FoodEvents;

public class FoodEntryUpdated(
    Guid foodEntryId,
    int calories,
    int protein,
    int carbs,
    int fat) : DomainEvent
{
    public Guid FoodEntryId { get; } = foodEntryId;
    public int Calories { get; } = calories;
    public int Protein { get; } = protein;
    public int Carbs { get; } = carbs;
    public int Fat { get; } = fat;
}