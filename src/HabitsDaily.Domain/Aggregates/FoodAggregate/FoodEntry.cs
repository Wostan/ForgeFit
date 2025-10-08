using HabitsDaily.Domain.Aggregates.UserAggregate;
using HabitsDaily.Domain.Enums;
using HabitsDaily.Domain.Exceptions;
using HabitsDaily.Domain.Primitives;
using HabitsDaily.Domain.ValueObjects;

namespace HabitsDaily.Domain.Aggregates.FoodAggregate;

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

    // Navigation properties
    public User User { get; private set; }
    public ICollection<FoodItem> FoodApiItems { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }

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

    public void AddFoodItem(FoodItem item)
    {
        if (item is null)
            throw new DomainValidationException("Food item cannot be null.");

        FoodApiItems.Add(item);
        RecalculateTotals();
    }

    public void RemoveFoodItem(string foodItemId)
    {
        var item = FoodApiItems.FirstOrDefault(f => f.ExternalId == foodItemId);
        if (item is null)
            throw new DomainValidationException("Food item not found.");

        FoodApiItems.Remove(item);
        RecalculateTotals();
    }

    private void RecalculateTotals()
    {
        Calories = FoodApiItems.Sum(i => i.Calories);
        Protein = FoodApiItems.Sum(i => i.Protein);
        Carbs = FoodApiItems.Sum(i => i.Carbs);
        Fat = FoodApiItems.Sum(i => i.Fat);

        UpdatedAt = DateTime.UtcNow;
    }
}