using HabitsDaily.Domain.Aggregates.StreakAggregate;
using HabitsDaily.Domain.Aggregates.UserAggregate;
using HabitsDaily.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HabitsDaily.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        
        // Properties
        builder.Property(u => u.Username)
            .HasMaxLength(20)
            .IsRequired();
        
        builder.Property(u => u.Email)
            .HasConversion<string>(
                v => v.Value, 
                v => new Email(v)
            )
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(u => u.DateOfBirth)
            .HasConversion(
                v => v.Value,
                v => new DateOfBirth(v)
            )
            .IsRequired();
        
        builder.Property(u => u.PasswordHash)
            .HasMaxLength(255)
            .IsRequired();
        
        builder.Property(u => u.AvatarUrl)
            .HasConversion(
                v => v != null ? v.ToString() : null,
                v => v != null ? new Uri(v) : null
            )
            .HasMaxLength(500);
        
        builder.Property(u => u.CreatedAt)
            .IsRequired();
        
        // Indexes
        builder.HasIndex(u => u.Username).IsUnique();
        builder.HasIndex(u => u.Email).IsUnique();
        
        // Navigation properties
        builder.HasMany(u => u.Habits)
            .WithOne(h => h.User)
            .HasForeignKey(h => h.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.ArchivedUserStats)
            .WithOne(au => au.User)
            .HasForeignKey(au => au.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(u => u.Streak)
            .WithOne(s => s.User)
            .HasForeignKey<Streak>(s => s.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.Posts)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Comments)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.Likes)
            .WithOne(l => l.User)
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.Purchases)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.Friends)
            .WithOne(f => f.User)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}