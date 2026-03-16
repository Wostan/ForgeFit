using ForgeFit.Domain.Constants;
using ForgeFit.Domain.Enums.ProfileEnums;
using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;

namespace ForgeFit.Domain.ValueObjects.UserValueObjects;

public class Height : ValueObject
{
    #region Constructors
    public Height(double value, HeightUnit unit)
    {
        SetHeightUnit(unit);
        SetHeight(value);
    }
    #endregion

    #region Public Properties
    public double Value { get; private set; }
    public HeightUnit Unit { get; private set; }
    #endregion

    #region Private Methods
    private void SetHeight(double value)
    {
        if (value <= 0)
            throw new DomainValidationException("Height must be greater than 0.");

        Value = Unit switch
        {
            HeightUnit.Cm when value is < DomainConstants.ValidationLimits.MinHeightCm
                or > DomainConstants.ValidationLimits.MaxHeightCm => throw new DomainValidationException(
                $"Height in cm must be between {DomainConstants.ValidationLimits.MinHeightCm} and {DomainConstants.ValidationLimits.MaxHeightCm}."),
            
            HeightUnit.Inch when value is < DomainConstants.ValidationLimits.MinHeightInches
                or > DomainConstants.ValidationLimits.MaxHeightInches => throw new DomainValidationException(
                $"Height in inches must be between {DomainConstants.ValidationLimits.MinHeightInches} and {DomainConstants.ValidationLimits.MaxHeightInches}."),
            _ => value
        };
    }

    private void SetHeightUnit(HeightUnit unit)
    {
        if (!Enum.IsDefined(unit))
            throw new DomainValidationException("HeightUnit is not defined.");

        Unit = unit;
    }
    #endregion

    #region Public Methods
    public Height ToCm()
    {
        return Unit == HeightUnit.Cm
            ? this
            : new Height(Value * DomainConstants.ConversionFactors.InchesToCm, HeightUnit.Cm);
    }

    public Height ToInches()
    {
        return Unit == HeightUnit.Inch
            ? this
            : new Height(Value * DomainConstants.ConversionFactors.CmToInches, HeightUnit.Inch);
    }

    public static Height FromCm(double cm) => new(cm, HeightUnit.Cm);
    public static Height FromInches(double inches) => new(inches, HeightUnit.Inch);

    public override string ToString() => $"{Value:F1} {Unit}";
    #endregion

    #region ValueObject Implementation
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
        yield return Unit;
    }
    #endregion
}
