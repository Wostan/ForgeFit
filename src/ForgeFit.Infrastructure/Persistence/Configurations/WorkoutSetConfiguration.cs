using ForgeFit.Domain.Aggregates.WorkoutAggregate;
using ForgeFit.Domain.Constants;
using ForgeFit.Domain.Enums.ProfileEnums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForgeFit.Infrastructure.Persistence.Configurations;

public class WorkoutSetConfiguration : IEntityTypeConfiguration<WorkoutSet>
{
    public void Configure(EntityTypeBuilder<WorkoutSet> builder)
    {
        builder.ToTable("WorkoutSets", tableBuilder =>
        {
            tableBuilder.HasCheckConstraint("CK_WorkoutSet_RepsCheck", $"Reps >= 1 AND Reps <= {DomainConstants.ValidationLimits.MaxRepsPerSet}");
            tableBuilder.HasCheckConstraint("CK_WorkoutSet_OrderCheck", "[Order] >= 0");
            tableBuilder.HasCheckConstraint("CK_WorkoutSets_WeightCheck", $"WeightValue >= 0 AND WeightValue < {DomainConstants.ValidationLimits.MaxWorkoutWeightKg}");
            tableBuilder.HasCheckConstraint("CK_WorkoutSets_WeightUnitCheck", $"WeightUnit IN ({string.Join(", ", Enum.GetValues<WeightUnit>().Cast<int>())})");
            tableBuilder.HasCheckConstraint("CK_WorkoutSets_RestTimeCheck", $"RestTime <= '00:{DomainConstants.ValidationLimits.MaxRestTimeMinutes}:00'");
        });

        builder.HasKey(ws => ws.Id);

        // Properties
        builder.Property(ws => ws.Id)
            .ValueGeneratedNever();

        builder.Property(ws => ws.UserId)
            .IsRequired();

        builder.Property(ws => ws.WorkoutExercisePlanId)
            .IsRequired();

        builder.Property(ws => ws.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();

        builder.Property(ws => ws.UpdatedAt)
            .IsRequired(false);

        // ValueObjects
        builder.OwnsOne(ws => ws.WorkoutSetInfo, workoutSetInfo =>
        {
            workoutSetInfo.Property(wsi => wsi.Order)
                .IsRequired()
                .HasColumnName("Order");

            workoutSetInfo.Property(wsi => wsi.Reps)
                .IsRequired()
                .HasColumnName("Reps");

            workoutSetInfo.OwnsOne(wsi => wsi.Weight, weight =>
            {
                weight.Property(w => w.Value)
                    .IsRequired()
                    .HasColumnName("WeightValue");

                weight.Property(w => w.Unit)
                    .IsRequired()
                    .HasColumnName("WeightUnit");
            });
        });

        builder.OwnsOne(ws => ws.RestTime, restTime =>
        {
            restTime.Property(rt => rt.Value)
                .IsRequired()
                .HasColumnName("RestTime");
        });

        // Indexes
        builder.HasIndex(ws => ws.UserId);
        builder.HasIndex(ws => ws.WorkoutExercisePlanId);

        // Navigation properties
        builder.HasOne(ws => ws.WorkoutExercisePlan)
            .WithMany(wep => wep.WorkoutSets)
            .HasForeignKey(ws => ws.WorkoutExercisePlanId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
