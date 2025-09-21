using HabitsDaily.Domain.Aggregates.StreakAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HabitsDaily.Infrastructure.Persistence.Configurations;

public class StreakConfiguration : IEntityTypeConfiguration<Streak>
{
    public void Configure(EntityTypeBuilder<Streak> builder)
    {
        builder.HasKey(s => s.Id);
        
        //Properties
        builder.Property(s => s.UserId)
            .IsRequired();
        
        builder.Property(s => s.Level)
            .HasConversion<int>()
            .IsRequired();
        
        builder.Property(s => s.CurrentStreakDays)
            .IsRequired();
        
        builder.Property(s => s.CreatedAt)
            .IsRequired();
        
        //Indexes
        builder.HasIndex(s => s.UserId).IsUnique();
        
        //Navigation properties
        builder.HasOne(s => s.User)
            .WithOne(u => u.Streak)
            .HasForeignKey<Streak>(s => s.UserId);
    }
}