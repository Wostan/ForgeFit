using ForgeFit.Domain.Aggregates.GoalAggregate;
using ForgeFit.Domain.Aggregates.UserAggregate;
using ForgeFit.Domain.Aggregates.WorkoutAggregate;
using Microsoft.EntityFrameworkCore;

namespace ForgeFit.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{ 
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<BodyGoal> BodyGoals { get; set; }
    public DbSet<NutritionGoal> NutritionGoals { get; set; }
    public DbSet<WorkoutGoal> WorkoutGoals { get; set; }
    public DbSet<WorkoutProgram> WorkoutPrograms { get; set; }
    public DbSet<WorkoutExercisePlan> WorkoutExercisePlans { get; set; }
    public DbSet<WorkoutSet> WorkoutSets { get; set; }
    public DbSet<WorkoutEntry> WorkoutEntries { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}