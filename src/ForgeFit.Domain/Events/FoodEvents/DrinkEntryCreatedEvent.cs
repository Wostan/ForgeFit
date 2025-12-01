using ForgeFit.Domain.Primitives;

namespace ForgeFit.Domain.Events.FoodEvents;

public class DrinkEntryCreatedEvent(
    Guid drinkEntryId,
    Guid userId,
    int volumeMl) : DomainEvent
{
    public Guid DrinkEntryId { get; } = drinkEntryId;
    public Guid UserId { get; } = userId;
    public int VolumeMl { get; } = volumeMl;
}
