using HabitsDaily.Domain.Aggregates.HabitAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HabitsDaily.Infrastructure.Persistence.Configurations;

public class HabitRecordConfiguration : IEntityTypeConfiguration<HabitRecord>
{
    public void Configure(EntityTypeBuilder<HabitRecord> builder)
    {
        builder.HasKey(hr => hr.Id);
        
        //Properties
        builder.Property(hr => hr.HabitId)
            .IsRequired();
        
        builder.Property(hr => hr.Date)
            .IsRequired();
        
        builder.Property(hr => hr.Completed)
            .IsRequired();
        
        builder.Property(hr => hr.PointsEarned)
            .IsRequired();
        
        //Indexes
        builder.HasIndex(hr => new { hr.HabitId, hr.Date }).IsUnique();

        //Navigation properties
        builder.HasOne(hr => hr.Habit)
            .WithMany(h => h.HabitRecords)
            .HasForeignKey(hr => hr.HabitId);
    }
}