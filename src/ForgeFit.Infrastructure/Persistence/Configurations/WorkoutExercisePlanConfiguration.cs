using System.Text.Json;
using ForgeFit.Domain.Aggregates.WorkoutAggregate;
using ForgeFit.Domain.Enums.WorkoutEnums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ForgeFit.Infrastructure.Persistence.Configurations;

public class WorkoutExercisePlanConfiguration : IEntityTypeConfiguration<WorkoutExercisePlan>
{
    public void Configure(EntityTypeBuilder<WorkoutExercisePlan> builder)
    {
        builder.ToTable("WorkoutExercisePlans");
        
        builder.HasKey(wep => wep.Id);

        // Properties
        builder.Property(wep => wep.CreatedAt)
            .IsRequired();

        // ValueObjects
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
                )
                .HasMaxLength(500);

            exercise.Property(e => e.TargetMuscles)
                .HasConversion(EnumListConverter<Muscle>())
                .IsRequired();

            exercise.Property(e => e.BodyParts)
                .HasConversion(EnumListConverter<BodyPart>())
                .IsRequired();

            exercise.Property(e => e.Equipment)
                .HasConversion(EnumListConverter<Equipment>())
                .IsRequired();
            
            exercise.Property(e => e.SecondaryMuscles)
                .HasConversion(EnumListConverter<Muscle>())
                .IsRequired();

            exercise.Property(e => e.Instructions)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) 
                         ?? new List<string>()
                )
                .HasMaxLength(2000);
            return;

            ValueConverter<IReadOnlyCollection<TEnum>, string> EnumListConverter<TEnum>() where TEnum : struct, Enum
            {
                return new ValueConverter<IReadOnlyCollection<TEnum>, string>(
                    v => string.Join(',', v), 
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(Enum.Parse<TEnum>).ToList()
                    );
            }
        });
        
        // Indexes
        builder.HasIndex(wep => wep.WorkoutProgramId);
        
        // Navigation properties
        builder.HasMany(wep => wep.WorkoutSets)
            .WithOne(ws => ws.WorkoutExercisePlan)
            .HasForeignKey("WorkoutExercisePlanId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}