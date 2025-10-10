using ForgeFit.Domain.Aggregates.GoalAggregate;
using ForgeFit.Domain.Aggregates.HabitAggregate;
using ForgeFit.Domain.Aggregates.UserAggregate;
using ForgeFit.Domain.Aggregates.WorkoutAggregate;
using ForgeFit.Domain.ValueObjects.FoodValueObjects;
using ForgeFit.Domain.ValueObjects.WorkoutValueObjects;
using Microsoft.EntityFrameworkCore;

namespace ForgeFit.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{ 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}