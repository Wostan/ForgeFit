using Microsoft.EntityFrameworkCore;

namespace HabitsDaily.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}