using ForgeFit.Domain.Aggregates.GoalAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForgeFit.Infrastructure.Persistence.Configurations;

public class WorkoutGoalConfiguration : IEntityTypeConfiguration<WorkoutGoal>
{
    [Obsolete("Obsolete")]
    public void Configure(EntityTypeBuilder<WorkoutGoal> builder)
    {
        builder.ToTable("WorkoutGoals")
            .HasCheckConstraint("CK_WorkoutGoals_WorkoutsPerWeekCheck", "WorkoutsPerWeek > 0 AND WorkoutsPerWeek < 8")
            .HasCheckConstraint("CK_WorkoutGoals_RecommendedWorkoutTypeCheck", "WorkoutType IN (1, 2, 3)")
            .HasCheckConstraint("CK_WorkoutGoals_DurationCheck", "RecommendedSchedule_Duration BETWEEN '00:10:00' AND '05:00:00'");
        
        builder.HasKey(wg => wg.Id);
        
        // Properties
        builder.Property(wg => wg.UserId)
            .IsRequired();
        
        builder.Property(wg => wg.WorkoutsPerWeek)
            .IsRequired();
        
        builder.Property(wg => wg.WorkoutType)
            .HasConversion<int>()
            .IsRequired();
        
        builder.Property(wg => wg.Duration)
            .IsRequired();

        builder.Property(wg => wg.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();
        
        // Indexes
        builder.HasIndex(wg => wg.UserId); 
        
        // Navigation properties
        builder.HasOne(wg => wg.User)
            .WithMany()
            .HasForeignKey(wg => wg.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}