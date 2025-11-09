using ForgeFit.Domain.Aggregates.UserAggregate;
using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;

namespace ForgeFit.Domain.Aggregates.GoalAggregate;

public class NutritionGoal : Entity, ITimeFields
{
    internal NutritionGoal(
        Guid userId,
        int calories,
        int carbs,
        int protein,
        int fat,
        int waterGoalMl)
    {
        SetUserId(userId);
        SetNutritionGoal(calories, protein, carbs, fat);
        SetWaterGoalMl(waterGoalMl);
        CreatedAt = DateTime.UtcNow;
    }

    private NutritionGoal()
    {
    }

    public Guid UserId { get; private set; }
    public int Calories { get; private set; }
    public int Carbs { get; private set; }
    public int Protein { get; private set; }
    public int Fat { get; private set; }
    public int WaterGoalMl { get; private set; }

    // Navigation properties
    public User User { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }

    private void SetUserId(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new DomainValidationException("WorkoutProgramId cannot be empty.");

        UserId = userId;
    }

    private void SetNutritionGoal(
        int calories,
        int protein,
        int carbs,
        int fat)
    {
        if (calories <= 0 || protein <= 0 || carbs <= 0 || fat <= 0)
            throw new DomainValidationException("CPF values must be greater than 0.");

        Calories = calories;
        Protein = protein;
        Carbs = carbs;
        Fat = fat;
    }

    private void SetWaterGoalMl(int waterGoalMl)
    {
        if (waterGoalMl <= 0)
            throw new DomainValidationException("Water goal must be greater than 0.");

        WaterGoalMl = waterGoalMl;
    }

    public void UpdateNutritionGoal(
        int calories,
        int protein,
        int carbs,
        int fat)
    {
        SetNutritionGoal(calories, protein, carbs, fat);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateWaterGoalMl(int waterGoalMl)
    {
        SetWaterGoalMl(waterGoalMl);
        UpdatedAt = DateTime.UtcNow;
    }
}