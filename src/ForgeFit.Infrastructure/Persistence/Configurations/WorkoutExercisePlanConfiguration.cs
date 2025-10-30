using ForgeFit.Domain.Aggregates.WorkoutAggregate;
using ForgeFit.Domain.Enums.WorkoutEnums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForgeFit.Infrastructure.Persistence.Configurations;

public class WorkoutExercisePlanConfiguration : IEntityTypeConfiguration<WorkoutExercisePlan>
{
[Obsolete("Obsolete")]
public void Configure(EntityTypeBuilder<WorkoutExercisePlan> builder)
    {
        builder.ToTable("WorkoutExercisePlans")
            .HasCheckConstraint("CK_WorkoutExercisePlan_SetsCheck", "Sets > 0")
            .HasCheckConstraint("CK_WorkoutExercisePlan_RepsCheck", "Reps > 0");
        
        builder.HasKey(wep => wep.Id);

        // Properties
        builder.Property(wep => wep.Sets)
            .IsRequired();

        builder.Property(wep => wep.Reps)
            .IsRequired();

        builder.Property(wep => wep.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();

        // Value objects
        builder.OwnsOne(wep => wep.WorkoutExercise, exercise =>
        {
            exercise.Property(e => e.ExternalId)
                .IsRequired()
                .HasMaxLength(100);

            exercise.Property(e => e.Name)
                .IsRequired()                
                .HasMaxLength(50);

            exercise.Property(e => e.GifUrl)
                .HasConversion(
                    v => v != null ? v.ToString() : null,
                    v => v != null ? new Uri(v) : null
                );

            exercise.Property(e => e.Instructions)
                .HasMaxLength(500);

            exercise.Property(e => e.TargetMuscles)
                .HasConversion(
                    v => string.Join(',', v.Select(x => x.ToString())),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                          .Select(Enum.Parse<Muscle>)
                          .ToList()
                )
                .IsRequired();

            exercise.Property(e => e.BodyParts)
                .HasConversion(
                    v => string.Join(',', v.Select(x => x.ToString())),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                          .Select(Enum.Parse<BodyPart>)
                          .ToList()
                )
                .IsRequired();

            exercise.Property(e => e.Equipment)
                .HasConversion(
                    v => string.Join(',', v.Select(x => x.ToString())),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                          .Select(Enum.Parse<Equipment>)
                          .ToList()
                )
                .IsRequired();
        });
        
        builder.OwnsOne(p => p.Weight, weight =>
        {
            weight.HasCheckConstraint("CK_WorkoutExercisePlan_Weight_WeightValueCheck", "WeightValue > 0")
                .HasCheckConstraint("CK_WorkoutExercisePlan_Weight_WeightUnitCheck", "WeightUnit IN (1, 2)");
            
            weight.Property(w => w.Value)
                .IsRequired()
                .HasColumnName("WeightValue");

            weight.Property(w => w.Unit)
                .IsRequired()
                .HasColumnName("WeightUnit");
        });
        
        // Indexes
        builder.HasIndex(wep => wep.WorkoutProgramId);
        
        // Navigation properties
        builder.HasOne(wep => wep.WorkoutProgram)
            .WithMany()
            .HasForeignKey(wep => wep.WorkoutProgramId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}