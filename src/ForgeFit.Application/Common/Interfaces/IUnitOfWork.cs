namespace ForgeFit.Application.Common.Interfaces;

public interface IUnitOfWork
{
    Task SaveChangesAsync();
}