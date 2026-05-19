using ForgeFit.Domain.Aggregates.UserAggregate;
using ForgeFit.Domain.Constants;
using ForgeFit.Domain.Enums.FoodEnums;
using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;
using ForgeFit.Domain.ValueObjects.FoodValueObjects;

namespace ForgeFit.Domain.Aggregates.FoodAggregate;

public class FoodEntry : Entity, ITimeFields
{
    #region Private Fields
    private readonly HashSet<FoodItem> _foodItems = [];
    #endregion

    #region Constructors
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
    #endregion

    #region Public Properties
    public Guid UserId { get; private set; }
    public NutritionInfo NutritionInfo { get; private set; }
    public DayTime DayTime { get; private set; }
    public DateTime Date { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    #endregion

    #region Navigation Properties
    public User User { get; private set; }
    public IReadOnlyCollection<FoodItem> FoodItems => _foodItems;
    #endregion

    #region Factory Methods
    public static FoodEntry Create(Guid userId, DayTime dayTime, DateTime date, HashSet<FoodItem> foodItems)
    {
        return new FoodEntry(userId, dayTime, date, foodItems);
    }
    #endregion

    #region Domain Methods
    public void Update(DayTime dayTime, DateTime date)
    {
        SetDayTime(dayTime);
        SetDate(date);
        UpdatedAt = DateTime.UtcNow;
    }


    public void UpdateFoodItems(HashSet<FoodItem> foodItems)
    {
        SetFoodItems(foodItems);
        UpdatedAt = DateTime.UtcNow;
    }
    #endregion

    #region Private Setters
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
        if (foodItems.Count > DomainConstants.ValidationLimits.MaxFoodItemsPerMeal)
            throw new DomainValidationException($"Cannot exceed {DomainConstants.ValidationLimits.MaxFoodItemsPerMeal} food items per meal");
        
        _foodItems.Clear();
        if (foodItems.Any(item => !_foodItems.Add(item)))
        {
            throw new DomainValidationException("Food item already exists.");
        }

        RecalculateNutritionInfo();
    }

    private void RecalculateNutritionInfo()
    {
        var totalNutrition = FoodItems.Aggregate(
            NutritionInfo.Zero,
            (total, item) => total + new NutritionInfo(
                item.Calories,
                item.Carbs,
                item.Protein,
                item.Fat,
                item.Fiber,
                item.Sugar,
                item.SaturatedFat,
                item.Sodium));

        NutritionInfo = totalNutrition;
    }
    #endregion
}
