using ForgeFit.Domain.Aggregates.GoalAggregate;
using ForgeFit.Domain.Constants;
using ForgeFit.Domain.Enums.WorkoutEnums;
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
                $"WorkoutPlan_WorkoutsPerWeek >= {DomainConstants.ValidationLimits.MinWorkoutsPerWeek} AND WorkoutPlan_WorkoutsPerWeek <= {DomainConstants.ValidationLimits.MaxWorkoutsPerWeek}");
            tableBuilder.HasCheckConstraint("CK_WorkoutGoals_WorkoutPlan_WorkoutTypeCheck",
                $"WorkoutPlan_WorkoutType IN ({string.Join(", ", Enum.GetValues<WorkoutType>().Cast<int>())})");
            tableBuilder.HasCheckConstraint("CK_WorkoutGoals_WorkoutPlan_DurationCheck",
                $"WorkoutPlan_Duration BETWEEN '00:0{DomainConstants.ValidationLimits.MinWorkoutDurationMinutes}:00' AND '0{DomainConstants.ValidationLimits.MaxWorkoutDurationHours}:00:00'");
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

        builder.Property(wg => wg.UpdatedAt)
            .IsRequired(false);

        // Indexes
        builder.HasIndex(wg => wg.UserId);

        // Navigation properties
        builder.HasOne(wg => wg.User)
            .WithMany()
            .HasForeignKey(wg => wg.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
