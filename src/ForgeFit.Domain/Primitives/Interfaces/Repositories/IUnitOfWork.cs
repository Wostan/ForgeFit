namespace ForgeFit.Domain.Primitives.Interfaces.Repositories;

public interface IUnitOfWork
{
    Task SaveChangesAsync();
}