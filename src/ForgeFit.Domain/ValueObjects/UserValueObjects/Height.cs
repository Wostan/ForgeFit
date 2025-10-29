using ForgeFit.Domain.Enums.ProfileEnums;
using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;

namespace ForgeFit.Domain.ValueObjects.UserValueObjects;

public class Height : ValueObject
{
    public Height(double value, HeightUnit unit)
    {
        SetHeight(value);
        SetHeightUnit(unit);
    }

    private Height()
    {
    }

    public double Value { get; private set; }
    public HeightUnit Unit { get; private set; }

    private void SetHeight(double value)
    {
        if (value <= 0)
            throw new DomainValidationException("Height must be greater than 0.");

        Value = value;
    }

    private void SetHeightUnit(HeightUnit unit)
    {
        if (!Enum.IsDefined(unit))
            throw new DomainValidationException("HeightUnit is not defined.");

        Unit = unit;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
        yield return Unit;
    }
}