using ForgeFit.Domain.Enums.ProfileEnums;
using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;

namespace ForgeFit.Domain.ValueObjects;

public class Weight : ValueObject
{
    public Weight(double value, WeightUnit unit)
    {
        SetWeight(value);
        SetWeightUnit(unit);
    }

    public double Value { get; private set; }
    public WeightUnit Unit { get; private set; }

    private void SetWeight(double weight)
    {
        if (weight <= 0)
            throw new DomainValidationException("Weight must be greater than 0.");

        Value = weight;
    }

    private void SetWeightUnit(WeightUnit unit)
    {
        if (!Enum.IsDefined(unit))
            throw new DomainValidationException("WeightUnit is not defined.");

        Unit = unit;
    }

    public Weight ToKg()
    {
        return Unit == WeightUnit.Kg
            ? this
            : new Weight(Value * 0.453592, WeightUnit.Kg);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
        yield return Unit;
    }
}