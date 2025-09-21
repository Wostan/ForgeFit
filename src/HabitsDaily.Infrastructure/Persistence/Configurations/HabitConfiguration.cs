using HabitsDaily.Domain.Aggregates.HabitAggregate;
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
        
        builder.Property(h => h.Frequency)
            .HasConversion<int>()
            .IsRequired();
        
        //Navigation properties TODO
    }
}