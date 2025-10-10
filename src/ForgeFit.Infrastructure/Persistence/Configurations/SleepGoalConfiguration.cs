using ForgeFit.Domain.Aggregates.GoalAggregate;
using ForgeFit.Domain.Enums.SleepEnums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForgeFit.Infrastructure.Persistence.Configurations;

public class SleepGoalConfiguration : IEntityTypeConfiguration<SleepGoal>
{
    [Obsolete("Obsolete")]
    public void Configure(EntityTypeBuilder<SleepGoal> builder)
    {
        builder.ToTable("SleepGoals");
        
        builder.HasKey(s => s.Id);
        
        // Properties
        builder.Property(s => s.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();
        
        builder.Property(sg => sg.Weekdays)
            .HasConversion(
                v => string.Join(',', v.Select(d => d.ToString())),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(Enum.Parse<Weekday>)
                    .ToHashSet()
            )
            .IsRequired();
        
        // ValueObject properties
        builder.OwnsOne(sg => sg.SleepSchedule, schedule =>
        {
            schedule.HasCheckConstraint("CK_SleepGoals_SleepSchedule_DurationCheck", "SleepSchedule_Duration BETWEEN '01:00:00' AND '14:00:00'");
            
            schedule.Property(s => s.Start)
                .HasColumnName("SleepSchedule_Bedtime")
                .IsRequired();
            
            schedule.Property(s => s.End)
                .HasColumnName("SleepSchedule_Waketime")
                .IsRequired();
            
            schedule.Property(s => s.Duration)
                .HasColumnName("SleepSchedule_Duration")
                .IsRequired();
        });
        
        // Indexes
        builder.HasIndex(s => s.UserId);
        
        // Navigation properties
        builder.HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}