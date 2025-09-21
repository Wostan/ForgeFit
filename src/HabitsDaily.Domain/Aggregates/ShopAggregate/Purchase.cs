using HabitsDaily.Domain.Exceptions;
using HabitsDaily.Domain.Primitives;

namespace HabitsDaily.Domain.Aggregates.ShopAggregate;

public class Purchase : EntityId, ITimeFields
{
    internal Purchase(
        Guid userId, 
        Guid shopItemId,
        int quantity,
        decimal totalPrice)
    {
        Id = Guid.NewGuid();
        SetUserId(userId);
        SetShopItemId(shopItemId);
        SetQuantity(quantity);
        SetTotalPrice(totalPrice);
        CreatedAt = DateTime.UtcNow;
    }

    private Purchase() { }

    public Guid UserId { get; private set; }
    public Guid ShopItemId { get; private set; }
    public int Quantity { get; private set; }
    public decimal TotalPrice { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }

    private void SetUserId(Guid userId)
    {
        if (userId == Guid.Empty) throw new DomainValidationException("UserId cannot be empty.");
        UserId = userId;
    }

    private void SetShopItemId(Guid shopItemId)
    {
        if (shopItemId == Guid.Empty) throw new DomainValidationException("ShopItemId cannot be empty.");
        ShopItemId = shopItemId;
    }

    private void SetQuantity(int quantity)
    {
        if (quantity <= 0) throw new DomainValidationException("Quantity must be positive.");
        Quantity = quantity;
    }

    private void SetTotalPrice(decimal price)
    {
        if (price < 0) throw new DomainValidationException("TotalPrice cannot be negative.");
        TotalPrice = price;
    }
}