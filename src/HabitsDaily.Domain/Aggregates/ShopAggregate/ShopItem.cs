using HabitsDaily.Domain.Exceptions;
using HabitsDaily.Domain.Primitives;

namespace HabitsDaily.Domain.Aggregates.ShopAggregate;

public class ShopItem : EntityId, ITimeFields
{
    internal ShopItem(
        string name, 
        string? description, 
        decimal price,
        ShopItemType type)
    {
        Id = Guid.NewGuid();
        SetName(name);
        SetDescription(description);
        SetPrice(price);
        SetType(type);
        CreatedAt = DateTime.UtcNow;
    }

    private ShopItem() { }

    public string Name { get; private set; }
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public ShopItemType Type { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public List<Purchase> Purchases { get; private set; } = [];

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainValidationException("Name cannot be empty.");
        if (name.Length > 200) throw new DomainValidationException("Name too long.");
        Name = name;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetDescription(string? description)
    {
        Description = description?.Trim();
        if (Description?.Length > 1000) throw new DomainValidationException("Description too long.");
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetPrice(decimal price)
    {
        if (price < 0) throw new DomainValidationException("Price cannot be negative.");
        Price = price;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void SetType(ShopItemType type)
    {
        if (!Enum.IsDefined(type)) throw new DomainValidationException("Invalid 'Type' value.");
        Type = type;
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum ShopItemType
{
    AvatarFrame = 1,
    Badge,
    Consumable
}