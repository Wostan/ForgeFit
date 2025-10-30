namespace ForgeFit.Domain.Primitives.Interfaces;

public interface IUnitOfWork
{
    Task SaveChangesAsync();
}