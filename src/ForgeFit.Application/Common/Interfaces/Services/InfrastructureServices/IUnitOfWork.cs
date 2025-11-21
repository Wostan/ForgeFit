namespace ForgeFit.Application.Common.Interfaces.Services.InfrastructureServices;

public interface IUnitOfWork
{
    Task SaveChangesAsync();
}