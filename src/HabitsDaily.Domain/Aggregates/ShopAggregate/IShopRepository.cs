namespace HabitsDaily.Domain.Aggregates.ShopAggregate;

public interface IShopRepository
{
    Task<ShopItem?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(ShopItem shopItem, CancellationToken ct = default);
    Task UpdateAsync(ShopItem shopItem, CancellationToken ct = default);
    Task DeleteAsync(ShopItem shopItem, CancellationToken ct = default);

    Task<IReadOnlyList<ShopItem>> GetAllAsync(int skip = 0, int take = 50, CancellationToken ct = default);
    Task<IReadOnlyList<ShopItem>> GetAllByTypeAsync(ShopItemType type, int skip = 0, int take = 50, CancellationToken ct = default);

    Task<Purchase?> GetPurchaseByIdAsync(Guid id, CancellationToken ct = default);
    Task AddPurchaseAsync(Purchase purchase, CancellationToken ct = default);
    Task UpdatePurchaseAsync(Purchase purchase, CancellationToken ct = default);
    Task DeletePurchaseAsync(Purchase purchase, CancellationToken ct = default);

    Task<IReadOnlyList<Purchase>> GetAllPurchasesAsync(int skip = 0, int take = 50, CancellationToken ct = default);
    Task<IReadOnlyList<Purchase>> GetAllPurchasesByUserIdAsync(Guid userId, int skip = 0, int take = 50, CancellationToken ct = default);

    Task<bool> ExistsShopItemAsync(Guid id, CancellationToken ct = default);
}