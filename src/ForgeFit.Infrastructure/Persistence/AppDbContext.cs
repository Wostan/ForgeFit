using ForgeFit.Domain.Aggregates.GoalAggregate;
using ForgeFit.Domain.Aggregates.UserAggregate;
using Microsoft.EntityFrameworkCore;

namespace ForgeFit.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{ 
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<BodyGoal> BodyGoals { get; set; }
    public DbSet<NutritionGoal> NutritionGoals { get; set; }
    public DbSet<WorkoutGoal> WorkoutGoals { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}