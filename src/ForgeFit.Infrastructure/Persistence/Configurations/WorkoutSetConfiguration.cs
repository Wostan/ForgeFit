using ForgeFit.Domain.Aggregates.WorkoutAggregate;
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
            tableBuilder.HasCheckConstraint("CK_WorkoutSet_RepsCheck", "Reps > 0");
            tableBuilder.HasCheckConstraint("CK_WorkoutSet_OrderCheck", "[Order] > 0");
            tableBuilder.HasCheckConstraint("CK_WorkoutSets_WeightCheck", "WeightValue > 0");
            tableBuilder.HasCheckConstraint("CK_WorkoutSets_WeightUnitCheck", "WeightUnit IN (1, 2)");
        });
        
        builder.HasKey(ws => ws.Id);

        // Properties
        builder.Property(ws => ws.Order)
            .IsRequired();

        builder.Property(ws => ws.Reps)
            .IsRequired();

        builder.Property(ws => ws.RestTime)
            .IsRequired();

        builder.Property(ws => ws.UserId)
            .IsRequired();

        // Value Objects
        builder.OwnsOne(ws => ws.Weight, weight =>
        { 
            weight.Property(w => w.Value)
                .IsRequired()
                .HasColumnName("WeightValue");

            weight.Property(w => w.Unit)
                .IsRequired()
                .HasColumnName("WeightUnit")
                .HasConversion(
                    v => v.ToString(),
                    v => Enum.Parse<WeightUnit>(v)
                );
        });
        
        // Navigation properties
        builder.HasOne(ws => ws.WorkoutExercisePlan)
            .WithMany(wep => wep.WorkoutSets)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
            
        // Indexes
        builder.HasIndex(ws => ws.UserId);
    }
}