using HabitsDaily.Domain.Exceptions;
using HabitsDaily.Domain.Primitives;

namespace HabitsDaily.Domain.ValueObjects;

public class FoodItem : ValueObject
{
    public FoodItem(
        string externalId,
        string label,
        int calories,
        int protein,
        int fat,
        int carbs
    )
    {
        SetExternalId(externalId);
        SetLabel(label);
        SetBfc(calories, protein, fat, carbs);
    }

    public string ExternalId { get; private set; }
    public string Label { get; private set; }
    public int Calories { get; private set; }
    public int Protein { get; private set; }
    public int Fat { get; private set; }
    public int Carbs { get; private set; }

    public void SetExternalId(string externalId)
    {
        if (string.IsNullOrWhiteSpace(externalId))
            throw new DomainValidationException("ExternalId cannot be null or whitespace");

        ExternalId = externalId;
    }

    public void SetLabel(string label)
    {
        if (string.IsNullOrWhiteSpace(label))
            throw new DomainValidationException("Name cannot be null or whitespace");

        if (label.Length > 20)
            throw new DomainValidationException("Name must be less than 20 characters long");

        Label = label;
    }

    public void SetBfc(int calories, int protein, int fat, int carbs)
    {
        if (calories < 0 || protein < 0 || fat < 0 || carbs < 0)
            throw new DomainValidationException("BFC values cannot be negative");

        Calories = calories;
        Protein = protein;
        Fat = fat;
        Carbs = carbs;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return ExternalId;
    }
}