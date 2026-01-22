using ForgeFit.Domain.Aggregates.WorkoutAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForgeFit.Infrastructure.Persistence.Configurations;

public class WorkoutProgramConfiguration : IEntityTypeConfiguration<WorkoutProgram>
{
    public void Configure(EntityTypeBuilder<WorkoutProgram> builder)
    {
        builder.ToTable("WorkoutPrograms");

        builder.HasKey(wp => wp.Id);

        // Properties
        builder.Property(wp => wp.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(wp => wp.Description)
            .HasMaxLength(300);

        builder.Property(wp => wp.CreatedAt)
            .IsRequired();

        builder.Property(wp => wp.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Filters
        builder.HasQueryFilter(wp => !wp.IsDeleted);

        // Indexes
        builder.HasIndex(wp => wp.UserId);

        // Navigation properties
        builder.HasOne(wp => wp.User)
            .WithMany()
            .HasForeignKey(wp => wp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(wp => wp.WorkoutExercisePlans)
            .WithOne(wep => wep.WorkoutProgram)
            .HasForeignKey(wep => wep.WorkoutProgramId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(wp => wp.WorkoutEntries)
            .WithOne(we => we.WorkoutProgram)
            .HasForeignKey(we => we.WorkoutProgramId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
