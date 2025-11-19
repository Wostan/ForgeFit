namespace ForgeFit.Infrastructure.Interfaces;

public interface IUnitOfWork
{
    Task SaveChangesAsync();
}