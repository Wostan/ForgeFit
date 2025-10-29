using ForgeFit.Domain.Aggregates.UserAggregate;
using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;

namespace ForgeFit.Domain.Aggregates.FoodAggregate;

public class DrinkEntry : Entity, ITimeFields
{
    internal DrinkEntry(
        Guid userId,
        int volumeMl)
    {
        SetUserId(userId);
        SetVolumeMl(volumeMl);
        CreatedAt = DateTime.UtcNow;
    }

    private DrinkEntry()
    {
    }

    public Guid UserId { get; private set; }
    public int VolumeMl { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public User User { get; private set; }

    private void SetUserId(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new DomainValidationException("WorkoutProgramId cannot be empty.");

        UserId = userId;
    }

    private void SetVolumeMl(int volumeMl)
    {
        if (volumeMl <= 0)
            throw new DomainValidationException("VolumeMl must be greater than 0.");

        VolumeMl = volumeMl;
    }
}