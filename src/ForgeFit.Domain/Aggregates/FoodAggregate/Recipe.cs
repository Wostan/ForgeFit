using ForgeFit.Domain.Aggregates.UserAggregate;
using ForgeFit.Domain.Constants;
using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;
using ForgeFit.Domain.ValueObjects.FoodValueObjects;

namespace ForgeFit.Domain.Aggregates.FoodAggregate;

public class Recipe : Entity, ITimeFields
{
    #region Private Fields
    private readonly HashSet<FoodItem> _ingredients = [];
    #endregion

    #region Constructors
    internal Recipe(
        Guid userId,
        string name,
        string? description,
        HashSet<FoodItem> ingredients)
    {
        SetUserId(userId);
        SetName(name);
        SetDescription(description);
        SetIngredients(ingredients);
        CreatedAt = DateTime.UtcNow;
    }

    private Recipe()
    {
    }
    #endregion

    #region Public Properties
    public Guid UserId { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    #endregion

    #region Navigation Properties
    public User User { get; private set; }
    public IReadOnlyCollection<FoodItem> Ingredients => _ingredients;
    #endregion

    #region Computed Properties
    public NutritionInfo TotalNutrition => _ingredients.Aggregate(
        NutritionInfo.Zero,
        (total, item) => total + new NutritionInfo(
            item.Calories,
            item.Carbs,
            item.Protein,
            item.Fat,
            item.Fiber,
            item.Sugar,
            item.SaturatedFat,
            item.Sodium));
    #endregion

    #region Factory Methods
    public static Recipe Create(
        Guid userId,
        string name,
        string? description,
        HashSet<FoodItem> ingredients)
    {
        return new Recipe(userId, name, description, ingredients);
    }
    #endregion

    #region Domain Methods
    public void UpdateDetails(string name, string? description)
    {
        SetName(name);
        SetDescription(description);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateIngredients(HashSet<FoodItem> ingredients)
    {
        SetIngredients(ingredients);
        UpdatedAt = DateTime.UtcNow;
    }
    #endregion

    #region Private Setters
    private void SetUserId(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new DomainValidationException("UserId cannot be empty.");

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

    private void SetDescription(string? description)
    {
        if (description is not null && description.Length > DomainConstants.ValidationLimits.MaxDescriptionLength)
            throw new DomainValidationException($"Description cannot exceed {DomainConstants.ValidationLimits.MaxDescriptionLength} characters");

        Description = description;
    }

    private void SetIngredients(HashSet<FoodItem> ingredients)
    {
        if (ingredients.Count > DomainConstants.ValidationLimits.MaxFoodItemsPerMeal)
            throw new DomainValidationException($"Cannot exceed {DomainConstants.ValidationLimits.MaxFoodItemsPerMeal} ingredients per recipe");

        _ingredients.Clear();
        foreach (var ingredient in ingredients)
        {
            if (!_ingredients.Add(ingredient))
            {
                throw new DomainValidationException("Duplicate ingredient found.");
            }
        }
    }
    #endregion
}
