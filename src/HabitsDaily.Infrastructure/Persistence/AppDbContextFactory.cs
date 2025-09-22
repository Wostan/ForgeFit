using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HabitsDaily.Infrastructure.Persistence;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer(
            "Server=localhost;Database=HabitsDaily;Trusted_Connection=True;TrustServerCertificate=True;",
            sql => sql.MigrationsAssembly("HabitsDaily.Infrastructure")
        );
        
        return new AppDbContext(optionsBuilder.Options);
    }
}