using ForgeFit.Domain.Aggregates.UserAggregate;
using ForgeFit.Domain.Constants;
using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;

namespace ForgeFit.Domain.Aggregates.FoodAggregate;

public class FoodProduct : Entity, ITimeFields
{
    #region Constructors
    internal FoodProduct(
        string? externalId,
        Guid? userId,
        string name,
        string? brand,
        string? barcode,
        double calories,
        double carbs,
        double protein,
        double fat,
        double fiber,
        double sugar,
        double saturatedFat,
        double sodium,
        double servingSize,
        string servingUnit)
    {
        SetExternalId(externalId);
        SetUserId(userId);
        SetName(name);
        SetBrand(brand);
        SetBarcode(barcode);
        SetNutrients(calories, carbs, protein, fat, fiber, sugar, saturatedFat, sodium);
        SetServingInfo(servingSize, servingUnit);
        CreatedAt = DateTime.UtcNow;
    }

    private FoodProduct()
    {
    }
    #endregion

    #region Public Properties
    public string? ExternalId { get; private set; }
    public Guid? UserId { get; private set; }
    public string Name { get; private set; }
    public string? Brand { get; private set; }
    public string? Barcode { get; private set; }
    public double Calories { get; private set; }
    public double Carbs { get; private set; }
    public double Protein { get; private set; }
    public double Fat { get; private set; }
    public double Fiber { get; private set; }
    public double Sugar { get; private set; }
    public double SaturatedFat { get; private set; }
    public double Sodium { get; private set; }
    public double ServingSize { get; private set; }
    public string ServingUnit { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    #endregion
    
    #region Navigation Properties
    public User? User { get; private set; }
    #endregion

    #region Factory Methods
    public static FoodProduct Create(
        string? externalId,
        Guid? userId,
        string name,
        string? brand,
        string? barcode,
        double calories,
        double carbs,
        double protein,
        double fat,
        double fiber,
        double sugar,
        double saturatedFat,
        double sodium,
        double servingSize,
        string servingUnit)
    {
        return new FoodProduct(
            externalId,
            userId,
            name,
            brand,
            barcode,
            calories,
            carbs,
            protein,
            fat,
            fiber,
            sugar,
            saturatedFat,
            sodium,
            servingSize,
            servingUnit);
    }
    #endregion

    #region Domain Methods
    public void UpdateDetails(string name, string? brand, string? barcode)
    {
        SetName(name);
        SetBrand(brand);
        SetBarcode(barcode);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateNutrients(
        double calories,
        double carbs,
        double protein,
        double fat,
        double fiber,
        double sugar,
        double saturatedFat,
        double sodium)
    {
        SetNutrients(calories, carbs, protein, fat, fiber, sugar, saturatedFat, sodium);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateServingInfo(double servingSize, string servingUnit)
    {
        SetServingInfo(servingSize, servingUnit);
        UpdatedAt = DateTime.UtcNow;
    }
    #endregion

    #region Private Setters
    private void SetExternalId(string? externalId)
    {
        if (externalId is not null && externalId.Length > DomainConstants.ValidationLimits.MaxExternalIdLength)
            throw new DomainValidationException($"ExternalId cannot exceed {DomainConstants.ValidationLimits.MaxExternalIdLength} characters");

        ExternalId = externalId;
    }

    private void SetUserId(Guid? userId)
    {
        UserId = userId;
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException("Name cannot be empty");

        if (name.Length > DomainConstants.ValidationLimits.MaxFoodLabelLength)
            throw new DomainValidationException($"Name cannot exceed {DomainConstants.ValidationLimits.MaxFoodLabelLength} characters");

        Name = name;
    }

    private void SetBrand(string? brand)
    {
        if (brand is not null && brand.Length > DomainConstants.ValidationLimits.MaxFoodLabelLength)
            throw new DomainValidationException($"Brand cannot exceed {DomainConstants.ValidationLimits.MaxFoodLabelLength} characters");

        Brand = brand;
    }

    private void SetBarcode(string? barcode)
    {
        if (barcode is not null && barcode.Length > DomainConstants.ValidationLimits.MaxBarcodeLength)
            throw new DomainValidationException($"Barcode cannot exceed {DomainConstants.ValidationLimits.MaxBarcodeLength} characters");

        Barcode = barcode;
    }

    private void SetNutrients(
        double calories,
        double carbs,
        double protein,
        double fat,
        double fiber,
        double sugar,
        double saturatedFat,
        double sodium)
    {
        ValidateNutrient(calories, "Calories");
        ValidateNutrient(carbs, "Carbs");
        ValidateNutrient(protein, "Protein");
        ValidateNutrient(fat, "Fat");
        ValidateNutrient(fiber, "Fiber");
        ValidateNutrient(sugar, "Sugar");
        ValidateNutrient(saturatedFat, "SaturatedFat");
        ValidateNutrient(sodium, "Sodium");

        Calories = calories;
        Carbs = carbs;
        Protein = protein;
        Fat = fat;
        Fiber = fiber;
        Sugar = sugar;
        SaturatedFat = saturatedFat;
        Sodium = sodium;
    }

    private void SetServingInfo(double servingSize, string servingUnit)
    {
        if (servingSize <= 0)
            throw new DomainValidationException("ServingSize must be greater than 0");

        if (string.IsNullOrWhiteSpace(servingUnit))
            throw new DomainValidationException("ServingUnit is required");

        if (servingUnit.Length > DomainConstants.ValidationLimits.MaxServingUnitLength)
            throw new DomainValidationException($"ServingUnit cannot exceed {DomainConstants.ValidationLimits.MaxServingUnitLength} characters");

        ServingSize = servingSize;
        ServingUnit = servingUnit;
    }

    private static void ValidateNutrient(double value, string nutrientName)
    {
        if (value < 0)
            throw new DomainValidationException($"{nutrientName} cannot be negative.");
    }
    #endregion
}
