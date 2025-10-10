using ForgeFit.Domain.Aggregates.UserAggregate;
using ForgeFit.Domain.Enums.FoodEnums;
using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;
using ForgeFit.Domain.ValueObjects.FoodValueObjects;

namespace ForgeFit.Domain.Aggregates.FoodAggregate;

public class FoodEntry : EntityId, ITimeFields
{
    internal FoodEntry(
        Guid userId,
        DayTime dayTime
    )
    {
        SetUserId(userId);
        SetDayTime(dayTime);
        CreatedAt = DateTime.UtcNow;
    }

    private FoodEntry()
    {
    }

    public Guid UserId { get; private set; }
    public int Calories { get; private set; }
    public int Carbs { get; private set; }
    public int Protein { get; private set; }
    public int Fat { get; private set; }
    public DayTime DayTime { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public User User { get; private set; }
    public HashSet<FoodItem> FoodItems { get; private set; }

    private void SetUserId(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new DomainValidationException("WorkoutProgramId cannot be empty.");

        UserId = userId;
    }

    private void SetDayTime(DayTime dayTime)
    {
        if (!Enum.IsDefined(dayTime))
            throw new DomainValidationException("DayTime is not defined.");

        DayTime = dayTime;
    }

    public void UpdateFoodItem(HashSet<FoodItem> foodItems)
    {
        FoodItems = foodItems ?? throw new DomainValidationException("Food items cannot be null.");
        RecalculateTotals();
    }

    public void RemoveFoodItem(string foodItemId)
    {
        var item = FoodItems.FirstOrDefault(f => f.ExternalId == foodItemId);
        if (item is null)
            throw new DomainValidationException("Food item not found.");

        FoodItems.Remove(item);
        RecalculateTotals();
    }

    private void RecalculateTotals()
    {
        Calories = FoodItems.Sum(i => i.Calories);
        Protein = FoodItems.Sum(i => i.Protein);
        Carbs = FoodItems.Sum(i => i.Carbs);
        Fat = FoodItems.Sum(i => i.Fat);

        UpdatedAt = DateTime.UtcNow;
    }
}