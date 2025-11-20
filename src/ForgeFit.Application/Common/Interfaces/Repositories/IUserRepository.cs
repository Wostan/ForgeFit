using ForgeFit.Domain.Aggregates.UserAggregate;

namespace ForgeFit.Application.Common.Interfaces.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> ExistsByEmailAsync(string email);
}