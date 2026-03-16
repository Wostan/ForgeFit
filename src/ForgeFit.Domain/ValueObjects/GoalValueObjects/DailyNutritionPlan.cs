using ForgeFit.Domain.Constants;
using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;

namespace ForgeFit.Domain.ValueObjects.GoalValueObjects;

public class DailyNutritionPlan : ValueObject
{
    #region Constructors
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
    #endregion

    #region Public Properties
    public int TargetCalories { get; private set; }
    public int Carbs { get; private set; }
    public int Protein { get; private set; }
    public int Fat { get; private set; }
    public int WaterMl { get; private set; }
    #endregion

    #region Private Methods
    private void SetTargetCalories(int targetCalories)
    {
        if (targetCalories is < DomainConstants.ValidationLimits.MinDailyCalories 
            or > DomainConstants.ValidationLimits.MaxDailyCalories)
            throw new DomainValidationException($"Target calories must be between {DomainConstants.ValidationLimits.MinDailyCalories} and {DomainConstants.ValidationLimits.MaxDailyCalories}");

        TargetCalories = targetCalories;
    }

    private void SetCarbs(int carbs)
    {
        if (carbs < 1)
            throw new DomainValidationException("Carbs must be positive");

        Carbs = carbs;
    }

    private void SetProtein(int protein)
    {
        if (protein < 1)
            throw new DomainValidationException("Protein must be positive");

        Protein = protein;
    }

    private void SetFat(int fat)
    {
        if (fat < 1)
            throw new DomainValidationException("Fat must be positive");

        Fat = fat;
    }

    private void SetWaterMl(int waterMl)
    {
        if (waterMl is < DomainConstants.ValidationLimits.MinWaterIntakeMl 
            or > DomainConstants.ValidationLimits.MaxWaterIntakeMl)
            throw new DomainValidationException($"Water must be between {DomainConstants.ValidationLimits.MinWaterIntakeMl}ml and {DomainConstants.ValidationLimits.MaxWaterIntakeMl}ml");

        WaterMl = waterMl;
    }
    #endregion

    #region Public Methods
    public static DailyNutritionPlan Create(int targetCalories, int carbs, int protein, int fat, int waterMl) 
        => new(targetCalories, carbs, protein, fat, waterMl);
    #endregion

    #region ValueObject Implementation
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return TargetCalories;
        yield return Carbs;
        yield return Protein;
        yield return Fat;
        yield return WaterMl;
    }
    #endregion
}
