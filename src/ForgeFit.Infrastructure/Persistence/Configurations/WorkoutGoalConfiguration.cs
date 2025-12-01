using ForgeFit.Domain.Aggregates.GoalAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForgeFit.Infrastructure.Persistence.Configurations;

public class WorkoutGoalConfiguration : IEntityTypeConfiguration<WorkoutGoal>
{
    public void Configure(EntityTypeBuilder<WorkoutGoal> builder)
    {
        builder.ToTable("WorkoutGoals", tableBuilder =>
        {
            tableBuilder.HasCheckConstraint("CK_WorkoutGoals_WorkoutPlan_WorkoutsPerWeekCheck",
                "WorkoutPlan_WorkoutsPerWeek > 0 AND WorkoutPlan_WorkoutsPerWeek < 8");
            tableBuilder.HasCheckConstraint("CK_WorkoutGoals_WorkoutPlan_WorkoutTypeCheck",
                "WorkoutPlan_WorkoutType IN (1, 2, 3)");
            tableBuilder.HasCheckConstraint("CK_WorkoutGoals_WorkoutPlan_DurationCheck",
                "WorkoutPlan_Duration BETWEEN '00:10:00' AND '05:00:00'");
        });

        builder.HasKey(wg => wg.Id);

        // Properties
        builder.Property(wg => wg.UserId)
            .IsRequired();

        // ValueObject properties
        builder.OwnsOne(wg => wg.WorkoutPlan, workoutPlan =>
        {
            workoutPlan.Property(wp => wp.WorkoutsPerWeek)
                .IsRequired();

            workoutPlan.Property(wp => wp.WorkoutType)
                .IsRequired();

            workoutPlan.Property(wp => wp.Duration)
                .IsRequired();
        });

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
