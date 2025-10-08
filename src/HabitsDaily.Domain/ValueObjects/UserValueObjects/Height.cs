using HabitsDaily.Domain.Enums;
using HabitsDaily.Domain.Exceptions;
using HabitsDaily.Domain.Primitives;

namespace HabitsDaily.Domain.ValueObjects;

public class Height : ValueObject
{
    private Height()
    {
    }

    public Height(double value, HeightUnit unit)
    {
        SetHeight(value);
        SetHeightUnit(unit);
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