using ForgeFit.Application.Common.Interfaces;
using ForgeFit.Infrastructure.Persistence;

namespace ForgeFit.Infrastructure.Services;

public class UnitOfWork(AppDbContext dbContext) : IUnitOfWork
{
    public async Task SaveChangesAsync()
    {
        await dbContext.SaveChangesAsync();
    }
}