using ForgeFit.Domain.Aggregates.NotificationAggregate;

namespace ForgeFit.Domain.Primitives.Interfaces.Repositories;

public interface INotificationRepository : IRepository<Notification>
{
    Task<List<Notification>> GetPendingByUserIdAsync(Guid userId);
}