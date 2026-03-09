using ForgeFit.Domain.Aggregates.UserAggregate;
using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;

namespace ForgeFit.Domain.Aggregates.FoodAggregate;

public class DrinkEntry : Entity, ITimeFields
{
    #region Private Fields
    #endregion

    #region Constructors
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
    #endregion

    #region Public Properties
    public Guid UserId { get; private set; }
    public int VolumeMl { get; private set; }
    public DateTime Date { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }
    #endregion

    #region Navigation Properties
    public User User { get; private set; }
    #endregion

    #region Factory Methods
    public static DrinkEntry Create(Guid userId, int volumeMl, DateTime date)
    {
        return new DrinkEntry(userId, volumeMl, date);
    }
    #endregion

    #region Domain Methods
    public void Update(int volumeMl, DateTime date)
    {
        SetVolumeMl(volumeMl);
        SetDate(date);
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
    #endregion
}
