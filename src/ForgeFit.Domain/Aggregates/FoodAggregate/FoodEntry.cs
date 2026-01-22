using ForgeFit.Domain.Aggregates.UserAggregate;
using ForgeFit.Domain.Enums.FoodEnums;
using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;
using ForgeFit.Domain.ValueObjects.FoodValueObjects;

namespace ForgeFit.Domain.Aggregates.FoodAggregate;

public class FoodEntry : Entity, ITimeFields
{
    private readonly HashSet<FoodItem> _foodItems = [];

    internal FoodEntry(
        Guid userId,
        DayTime dayTime,
        DateTime date,
        HashSet<FoodItem> foodItems
    )
    {
        SetUserId(userId);
        SetDayTime(dayTime);
        SetDate(date);
        SetFoodItems(foodItems);
        CreatedAt = DateTime.UtcNow;
    }

    private FoodEntry()
    {
    }

    public Guid UserId { get; private set; }
    public double Calories { get; private set; }
    public double Carbs { get; private set; }
    public double Protein { get; private set; }
    public double Fat { get; private set; }
    public DayTime DayTime { get; private set; }
    public DateTime Date { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public User User { get; private set; }
    public IReadOnlyCollection<FoodItem> FoodItems => _foodItems;

    public static FoodEntry Create(Guid userId, DayTime dayTime, DateTime date, HashSet<FoodItem> foodItems)
    {
        return new FoodEntry(userId, dayTime, date, foodItems);
    }

    private void SetUserId(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new DomainValidationException("UserId cannot be empty.");

        UserId = userId;
    }

    private void SetDayTime(DayTime dayTime)
    {
        if (!Enum.IsDefined(dayTime))
            throw new DomainValidationException("DayTime is not defined.");

        DayTime = dayTime;
    }

    private void SetDate(DateTime date)
    {
        Date = date;
    }

    private void SetFoodItems(HashSet<FoodItem> foodItems)
    {
        _foodItems.Clear();

        foreach (var item in foodItems) _foodItems.Add(item);

        RecalculateTotals();
    }

    public void Update(DayTime dayTime, DateTime date, HashSet<FoodItem> foodItems)
    {
        SetDayTime(dayTime);
        SetDate(date);
        SetFoodItems(foodItems);
    }

    private void RecalculateTotals()
    {
        Calories = FoodItems.Sum(i => i.Calories);
        Carbs = FoodItems.Sum(i => i.Carbs);
        Protein = FoodItems.Sum(i => i.Protein);
        Fat = FoodItems.Sum(i => i.Fat);

        UpdatedAt = DateTime.UtcNow;
    }
}
