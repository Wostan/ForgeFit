using ForgeFit.Domain.Aggregates.HabitAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForgeFit.Infrastructure.Persistence.Configurations;

public class HabitConfiguration : IEntityTypeConfiguration<Habit>
{
    [Obsolete("Obsolete")]
    public void Configure(EntityTypeBuilder<Habit> builder)
    {
        builder.ToTable("Habits");
        
        builder.HasKey(h => h.Id);
        
        // Properties
        builder.Property(h => h.Name)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(h => h.Description)
            .HasMaxLength(300);
        
        builder.Property(h => h.IsActive)
            .HasDefaultValue(true)
            .IsRequired();
        
        builder.Property(h => h.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();
        
        // ValueObject properties
        builder.OwnsOne(h => h.Frequency, freq =>
        {
            freq.HasCheckConstraint("CK_Habits_Frequency_FrequencyUnitCheck", "Frequency_FrequencyUnit IN (1, 2, 3)")
                .HasCheckConstraint("CK_Habits_Frequency_IntervalCheck", "Frequency_Interval > 0");
            
            freq.Property(f => f.FrequencyUnit)
                .HasConversion<int>()
                .IsRequired();
            
            freq.Property(f => f.Interval)
                .HasDefaultValue(1)
                .IsRequired();
        });
        
        // Indexes
        builder.HasIndex(h => h.UserId);
        
        // Navigation properties
        builder.HasOne(h => h.User)
            .WithMany()
            .HasForeignKey(h => h.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(h => h.HabitRecords)
            .WithOne(hr => hr.Habit)
            .HasForeignKey(hr => hr.HabitId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}