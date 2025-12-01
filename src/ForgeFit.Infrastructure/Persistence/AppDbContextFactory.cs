using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ForgeFit.Infrastructure.Persistence;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer(
            "Server=localhost;Database=ForgeFit;Trusted_Connection=True;TrustServerCertificate=True;",
            sql => sql.MigrationsAssembly("ForgeFit.Infrastructure")
        );

        return new AppDbContext(optionsBuilder.Options);
    }
}
