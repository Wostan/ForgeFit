using ForgeFit.Domain.Primitives;

namespace ForgeFit.Domain.ValueObjects.GoalValueObjects;

public class DailyNutritionPlan : ValueObject
{
    public DailyNutritionPlan(
        int targetCalories,
        int carbs,
        int protein,
        int fat,
        int waterMl)
    {
        SetTargetCalories(targetCalories);
        SetCarbs(carbs);
        SetProtein(protein);
        SetFat(fat);
        SetWaterMl(waterMl);
    }

    public int TargetCalories { get; private set; }
    public int Carbs { get; private set; }
    public int Protein { get; private set; }
    public int Fat { get; private set; }
    public int WaterMl { get; private set; }

    private void SetTargetCalories(int targetCalories)
    {
        if (targetCalories < 500)
            throw new ArgumentException("Target calories cannot be less than 500");

        TargetCalories = targetCalories;
    }

    private void SetCarbs(int carbs)
    {
        if (carbs < 1)
            throw new ArgumentException("Carbs must be positive");

        Carbs = carbs;
    }

    private void SetProtein(int protein)
    {
        if (protein < 1)
            throw new ArgumentException("Protein must be positive");

        Protein = protein;
    }

    private void SetFat(int fat)
    {
        if (fat < 1)
            throw new ArgumentException("Fat must be positive");

        Fat = fat;
    }

    private void SetWaterMl(int waterMl)
    {
        if (waterMl < 1000)
            throw new ArgumentException("Water cannot be less than 1000");

        WaterMl = waterMl;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return TargetCalories;
        yield return Carbs;
        yield return Protein;
        yield return Fat;
        yield return WaterMl;
    }
}
