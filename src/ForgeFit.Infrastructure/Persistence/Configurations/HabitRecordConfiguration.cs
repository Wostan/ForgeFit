using ForgeFit.Domain.Aggregates.HabitAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForgeFit.Infrastructure.Persistence.Configurations;

public class HabitRecordConfiguration : IEntityTypeConfiguration<HabitRecord>
{
    public void Configure(EntityTypeBuilder<HabitRecord> builder)
    {
        builder.ToTable("HabitRecords");
        
        builder.HasKey(hr => hr.Id);
        
        // Properties
        builder.Property(hr => hr.DueDate)
            .IsRequired();
        
        builder.Property(hr => hr.Completed)
            .HasDefaultValue(false)
            .IsRequired();
        
        // Indexes
        builder.HasIndex(hr => hr.HabitId);
        
        // Navigation properties
        builder.HasOne(hr => hr.Habit)
            .WithMany(h => h.HabitRecords)
            .HasForeignKey(hr => hr.HabitId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}