using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;

namespace ForgeFit.Domain.ValueObjects.FoodValueObjects;

public class NutritionInfo : ValueObject
{
    #region Constructors
    public NutritionInfo(double calories, double carbs, double protein, double fat)
    {
        SetCalories(calories);
        SetCarbs(carbs);
        SetProtein(protein);
        SetFat(fat);
    }
    
    private NutritionInfo() { }
    #endregion

    #region Public Properties
    public double Calories { get; private set; }
    public double Carbs { get; private set; }
    public double Protein { get; private set; }
    public double Fat { get; private set; }
    #endregion

    #region Private Methods
    private static void ValidateNutrient(double value, string nutrientName)
    {
        if (value < 0)
            throw new DomainValidationException($"{nutrientName} cannot be negative.");
    }

    private void SetCalories(double calories)
    {
        ValidateNutrient(calories, "Calories");
        Calories = calories;
    }

    private void SetCarbs(double carbs)
    {
        ValidateNutrient(carbs, "Carbs");
        Carbs = carbs;
    }

    private void SetProtein(double protein)
    {
        ValidateNutrient(protein, "Protein");
        Protein = protein;
    }

    private void SetFat(double fat)
    {
        ValidateNutrient(fat, "Fat");
        Fat = fat;
    }
    #endregion

    #region Public Methods
    public static NutritionInfo operator +(NutritionInfo left, NutritionInfo right)
    {
        return new NutritionInfo(
            left.Calories + right.Calories,
            left.Carbs + right.Carbs,
            left.Protein + right.Protein,
            left.Fat + right.Fat);
    }

    public static NutritionInfo Zero => new(0, 0, 0, 0);

    public double TotalMacronutrients => Carbs + Protein + Fat;
    
    public override string ToString() => $"C:{Calories:F0} | P:{Protein:F1}g | C:{Carbs:F1}g | F:{Fat:F1}g";
    #endregion

    #region ValueObject Implementation
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Calories;
        yield return Carbs;
        yield return Protein;
        yield return Fat;
    }
    #endregion
}
