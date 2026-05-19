using ForgeFit.Domain.Constants;
using ForgeFit.Domain.Enums.ProfileEnums;
using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;

namespace ForgeFit.Domain.ValueObjects;

public class Weight : ValueObject
{
    #region Constructors
    public Weight(double value, WeightUnit unit)
    {
        SetWeightUnit(unit);
        SetWeight(value);
    }
    #endregion

    #region Public Properties
    public double Value { get; private set; }
    public WeightUnit Unit { get; private set; }
    #endregion

    #region Private Methods
    private void SetWeight(double weight)
    {
        if (weight < 0)
            throw new DomainValidationException("Weight must be positive.");

        Value = weight;
    }

    private void SetWeightUnit(WeightUnit unit)
    {
        if (!Enum.IsDefined(unit))
            throw new DomainValidationException("WeightUnit is not defined.");

        Unit = unit;
    }
    #endregion

    #region Public Methods
    public Weight ToKg()
    {
        return Unit == WeightUnit.Kg
            ? this
            : new Weight(Value * DomainConstants.ConversionFactors.LbsToKg, WeightUnit.Kg);
    }

    public Weight ToLbs()
    {
        return Unit == WeightUnit.Lb
            ? this
            : new Weight(Value * DomainConstants.ConversionFactors.KgToLbs, WeightUnit.Lb);
    }

    public static Weight FromKg(double kg) => new(kg, WeightUnit.Kg);
    public static Weight FromLbs(double lbs) => new(lbs, WeightUnit.Lb);

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
