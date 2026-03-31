using ForgeFit.Domain.Constants;
using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;

namespace ForgeFit.Domain.ValueObjects.FoodValueObjects;

public class FoodItem : ValueObject
{
    #region Constructors
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
    #endregion

    #region Public Properties
    public string ExternalId { get; private set; }
    public string Label { get; private set; }
    public double Calories { get; private set; }
    public double Carbs { get; private set; }
    public double Protein { get; private set; }
    public double Fat { get; private set; }
    public string ServingUnit { get; private set; }
    public double Amount { get; private set; }
    #endregion

    #region Private Methods
    private void SetExternalId(string externalId)
    {
        if (string.IsNullOrWhiteSpace(externalId))
            throw new DomainValidationException("ExternalId cannot be empty");
        
        if (externalId.Length > DomainConstants.ValidationLimits.MaxExternalIdLength)
            throw new DomainValidationException($"ExternalId cannot exceed {DomainConstants.ValidationLimits.MaxExternalIdLength} characters");
            
        ExternalId = externalId;
    }

    private void SetLabel(string label)
    {
        if (string.IsNullOrWhiteSpace(label))
            throw new DomainValidationException("Name cannot be empty");
        if (label.Length > DomainConstants.ValidationLimits.MaxFoodLabelLength)
            throw new DomainValidationException($"Label cannot exceed {DomainConstants.ValidationLimits.MaxFoodLabelLength} characters");
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
        if (amount is < DomainConstants.ValidationLimits.MinFoodAmount or > DomainConstants.ValidationLimits.MaxFoodAmount)
            throw new DomainValidationException($"Amount must be between {DomainConstants.ValidationLimits.MinFoodAmount} and {DomainConstants.ValidationLimits.MaxFoodAmount}");

        ServingUnit = servingUnit;
        Amount = amount;
    }
    #endregion

    #region ValueObject Implementation
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return ExternalId;
        yield return ServingUnit;
        yield return Amount;
    }
    #endregion
}
