using System.Text.Json;
using ForgeFit.Domain.Aggregates.WorkoutAggregate;
using ForgeFit.Domain.Constants;
using ForgeFit.Domain.Enums.WorkoutEnums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ForgeFit.Infrastructure.Persistence.Configurations;

public class WorkoutEntryConfiguration : IEntityTypeConfiguration<WorkoutEntry>
{
    public void Configure(EntityTypeBuilder<WorkoutEntry> builder)
    {
        builder.ToTable("WorkoutEntries", tableBuilder =>
        {
            tableBuilder.HasCheckConstraint("CK_WorkoutEntries_WorkoutSchedule_DurationCheck",
                $"WorkoutSchedule_Duration BETWEEN '00:0{DomainConstants.ValidationLimits.MinWorkoutDurationMinutes}:00' AND '0{DomainConstants.ValidationLimits.MaxWorkoutDurationHours}:00:00'");
        });

        builder.HasKey(we => we.Id);

        // Properties
        builder.Property(we => we.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();

        builder.Property(we => we.UpdatedAt)
            .IsRequired(false);

        // ValueObject properties
        builder.OwnsOne(we => we.WorkoutSchedule, schedule =>
        {
            schedule.Property(s => s.Start)
                .IsRequired();

            schedule.Property(s => s.End)
                .IsRequired();

            schedule.Property(s => s.Duration)
                .IsRequired();
        });

        builder.OwnsMany(we => we.PerformedExercises, performedExercises =>
        {
            performedExercises.ToTable("PerformedExercises");

            performedExercises.WithOwner()
                .HasForeignKey("WorkoutEntryId");

            performedExercises.Property<int>("Id");
            performedExercises.HasKey("WorkoutEntryId", "Id");

            performedExercises.OwnsOne(pe => pe.Snapshot, snap =>
            {
                snap.Property(e => e.ExternalId)
                    .IsRequired()
                    .HasMaxLength(DomainConstants.ValidationLimits.MaxExternalIdLength);

                snap.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(DomainConstants.ValidationLimits.MaxExerciseNameLength);

                snap.Property(e => e.GifUrl)
                    .HasConversion(
                        v => v != null ? v.ToString() : null,
                        v => v != null ? new Uri(v) : null)
                    .HasMaxLength(DomainConstants.ValidationLimits.MaxExerciseGifUrlLength);

                snap.Property(e => e.TargetMuscles)
                    .HasConversion(EnumListConverter<Muscle>())
                    .IsRequired();

                snap.Property(e => e.BodyParts)
                    .HasConversion(EnumListConverter<BodyPart>())
                    .IsRequired();

                snap.Property(e => e.Equipment)
                    .HasConversion(EnumListConverter<Equipment>())
                    .IsRequired();

                snap.Property(e => e.SecondaryMuscles)
                    .HasConversion(EnumListConverter<Muscle>())
                    .IsRequired();

                snap.Property(e => e.Instructions)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                        v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null)
                             ?? new List<string>()
                    ).HasMaxLength(DomainConstants.ValidationLimits.MaxExerciseInstructionsLength);
                return;

                ValueConverter<IReadOnlyCollection<TEnum>, string> EnumListConverter<TEnum>() where TEnum : struct, Enum
                {
                    return new ValueConverter<IReadOnlyCollection<TEnum>, string>(
                        v => string.Join(',', v),
                        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(Enum.Parse<TEnum>).ToList()
                    );
                }
            });

            performedExercises.OwnsMany(pe => pe.Sets, sets =>
            {
                sets.ToTable("PerformedSets");

                sets.WithOwner()
                    .HasForeignKey("WorkoutEntryId", "PerformedExerciseId");

                sets.Property<int>("Id");
                sets.HasKey("WorkoutEntryId", "PerformedExerciseId", "Id");

                sets.Property(s => s.Order)
                    .IsRequired();

                sets.Property(s => s.Reps)
                    .IsRequired();

                sets.Property(s => s.IsCompleted)
                    .IsRequired();

                sets.OwnsOne(p => p.Weight, weight =>
                {
                    weight.Property(w => w.Value)
                        .IsRequired()
                        .HasColumnName("WeightValue");

                    weight.Property(w => w.Unit)
                        .IsRequired()
                        .HasColumnName("WeightUnit");
                });
            });
        });

        // Indexes
        builder.HasIndex(we => we.UserId);
        builder.HasIndex(we => we.WorkoutProgramId);

        // Navigation properties
        builder.HasOne(we => we.User)
            .WithMany()
            .HasForeignKey(we => we.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(we => we.WorkoutProgram)
            .WithMany()
            .HasForeignKey(we => we.WorkoutProgramId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
