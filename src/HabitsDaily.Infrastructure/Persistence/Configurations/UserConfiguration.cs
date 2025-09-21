using HabitsDaily.Domain.Aggregates.UserAggregate;
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
            .HasMaxLength(100)
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
        
        // Navigation properties TODO
    }
}