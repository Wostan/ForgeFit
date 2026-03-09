using ForgeFit.Domain.Aggregates.UserAggregate;
using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;
using ForgeFit.Domain.ValueObjects.GoalValueObjects;

namespace ForgeFit.Domain.Aggregates.GoalAggregate;

public class NutritionGoal : Entity, ITimeFields
{
    #region Private Fields
    #endregion

    #region Constructors
    internal NutritionGoal(Guid userId, DailyNutritionPlan dailyNutritionPlan)
    {
        SetUserId(userId);
        SetDailyNutritionPlan(dailyNutritionPlan);
        CreatedAt = DateTime.UtcNow;
    }

    private NutritionGoal()
    {
    }
    #endregion

    #region Public Properties
    public Guid UserId { get; private set; }
    public DailyNutritionPlan DailyNutritionPlan { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }
    #endregion

    #region Navigation Properties
    public User User { get; private set; }
    #endregion

    #region Factory Methods
    public static NutritionGoal Create(Guid userId, DailyNutritionPlan dailyNutritionPlan)
    {
        return new NutritionGoal(userId, dailyNutritionPlan);
    }
    #endregion

    #region Domain Methods
    public void Update(
        int calories,
        int carbs,
        int protein,
        int fat,
        int waterGoalMl)
    {
        SetDailyNutritionPlan(new DailyNutritionPlan(calories, carbs, protein, fat, waterGoalMl));
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

    private void SetDailyNutritionPlan(DailyNutritionPlan dailyNutritionPlan)
    {
        DailyNutritionPlan = dailyNutritionPlan ??
                             throw new DomainValidationException("DailyNutritionPlan cannot be null.");
    }
    #endregion
}
