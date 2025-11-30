using ForgeFit.Domain.Aggregates.UserAggregate;
using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;

namespace ForgeFit.Domain.Aggregates.FoodAggregate;

public class DrinkEntry : Entity, ITimeFields
{
    internal DrinkEntry(Guid userId, int volumeMl, DateTime date)
    {
        SetUserId(userId);
        SetVolumeMl(volumeMl);
        SetDate(date);
        CreatedAt = DateTime.UtcNow;
    }

    private DrinkEntry()
    {
    }

    public Guid UserId { get; private set; }
    public int VolumeMl { get; private set; }
    public DateTime Date { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public User User { get; private set; }

    public static DrinkEntry Create(Guid userId, int volumeMl, DateTime date)
    {
        return new DrinkEntry(userId, volumeMl, date);
    }

    private void SetUserId(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new DomainValidationException("UserId cannot be empty.");

        UserId = userId;
    }

    private void SetVolumeMl(int volumeMl)
    {
        if (volumeMl <= 0)
            throw new DomainValidationException("VolumeMl must be greater than 0.");

        VolumeMl = volumeMl;
    }
    
    private void SetDate(DateTime date)
    { 
        Date = date;
    }
    
    public void Update(int volumeMl, DateTime date)
    {
        SetVolumeMl(volumeMl);
        SetDate(date);
        UpdatedAt = DateTime.UtcNow;
    }
}