using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;

namespace ForgeFit.Domain.ValueObjects.FoodValueObjects;

public class FoodItem : ValueObject
{
    public FoodItem(
        string externalId,
        string label,
        double calories,
        double carbs,
        double protein,
        double fat,
        string servingUnit,
        double amount
    )
    {
        SetExternalId(externalId);
        SetLabel(label);
        SetBfc(calories, carbs, protein, fat);
        SetServingInfo(servingUnit, amount);
    }

    private FoodItem() { }

    public string ExternalId { get; private set; }
    public string Label { get; private set; }
    public double Calories { get; private set; }
    public double Carbs { get; private set; }
    public double Protein { get; private set; }
    public double Fat { get; private set; }
    public string ServingUnit { get; private set; } 
    public double Amount { get; private set; }

    private void SetExternalId(string externalId)
    {
        if (string.IsNullOrWhiteSpace(externalId))
            throw new DomainValidationException("ExternalId cannot be empty");
        ExternalId = externalId;
    }

    private void SetLabel(string label)
    {
        if (string.IsNullOrWhiteSpace(label))
            throw new DomainValidationException("Name cannot be empty");
        Label = label;
    }

    private void SetBfc(double calories, double carbs, double protein, double fat)
    {
        if (calories < 0 || carbs < 0 || protein < 0 || fat < 0)
            throw new DomainValidationException("Nutrients cannot be negative");
        
        Calories = calories;
        Carbs = carbs;
        Protein = protein;
        Fat = fat;
    }

    private void SetServingInfo(string servingUnit, double amount)
    {
        if (string.IsNullOrWhiteSpace(servingUnit))
            throw new DomainValidationException("Serving unit is required");
        if (amount <= 0)
            throw new DomainValidationException("Amount must be greater than 0");

        ServingUnit = servingUnit;
        Amount = amount;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return ExternalId;
        yield return ServingUnit;
        yield return Amount;
    }
}