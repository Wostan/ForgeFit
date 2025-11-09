using ForgeFit.Domain.Primitives;

namespace ForgeFit.Domain.Events.FoodEvents;

public class DrinkEntryUpdatedEvent(
    Guid drinkEntryId, 
    int volumeMl) : DomainEvent
{
    public Guid DrinkEntryId { get; } = drinkEntryId;
    public int VolumeMl { get; } = volumeMl;
}