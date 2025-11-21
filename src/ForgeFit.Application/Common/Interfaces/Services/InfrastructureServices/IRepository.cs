using ForgeFit.Domain.Primitives;

namespace ForgeFit.Application.Common.Interfaces.Services.InfrastructureServices;

public interface IRepository<T> where T : Entity
{
    Task AddAsync(T entity);
    Task<T?> GetByIdAsync(Guid id);
    Task<List<T>> GetAllAsync();
    Task<bool> ExistsAsync(Guid id);
    void Remove(T entity);
}