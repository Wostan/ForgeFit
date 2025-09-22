using HabitsDaily.Domain.Aggregates.HabitAggregate;
using HabitsDaily.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HabitsDaily.Infrastructure.Persistence.Configurations;

public class HabitConfiguration : IEntityTypeConfiguration<Habit>
{
    public void Configure(EntityTypeBuilder<Habit> builder)
    {
        builder.HasKey(h => h.Id);
        
        //Properties
        builder.Property(h => h.UserId)
            .IsRequired();

        builder.Property(h => h.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(h => h.Description)
            .HasMaxLength(300);
        
        builder.OwnsOne(h => h.Frequency, freq =>
        {
            freq.Property(f => f.Interval)
                .HasColumnName("FrequencyInterval")
                .IsRequired();

            freq.Property(f => f.Unit)
                .HasConversion<int>()
                .HasColumnName("FrequencyUnit")
                .IsRequired();
        });
        
        //Navigation properties
        builder.HasOne(h => h.User)
            .WithMany(u => u.Habits)
            .HasForeignKey(h => h.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(h => h.HabitRecords)
            .WithOne(hr => hr.Habit)
            .HasForeignKey(hr => hr.HabitId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(h => h.ArchivedUserStats)
            .WithOne(au => au.Habit)
            .HasForeignKey(au => au.HabitId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}