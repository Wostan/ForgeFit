using HabitsDaily.Domain.Aggregates.HabitAggregate;
using HabitsDaily.Domain.Aggregates.PostAggregate;
using HabitsDaily.Domain.Aggregates.ShopAggregate;
using HabitsDaily.Domain.Aggregates.StreakAggregate;
using HabitsDaily.Domain.Aggregates.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HabitsDaily.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Friend> Friends { get; set; }
    public DbSet<Habit> Habits { get; set; }
    public DbSet<HabitRecord> HabitRecords { get; set; }
    public DbSet<ArchivedUserStats> ArchivedUserStats { get; set; }
    public DbSet<Streak> Streaks { get; set; }
    public DbSet<ShopItem> ShopItems { get; set; }
    public DbSet<Purchase> Purchases { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Like> Like { get; set; }
    public DbSet<Comment> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}