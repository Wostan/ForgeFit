using ForgeFit.Domain.Aggregates.WorkoutAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForgeFit.Infrastructure.Persistence.Configurations;

public class WorkoutEntryConfiguration : IEntityTypeConfiguration<WorkoutEntry>
{
    [Obsolete("Obsolete")]
    public void Configure(EntityTypeBuilder<WorkoutEntry> builder)
    {
        builder.ToTable("WorkoutEntries");
        
        builder.HasKey(we => we.Id);
        
        // Properties
        builder.Property(we => we.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();
        
        // ValueObject properties
        builder.OwnsOne(we => we.WorkoutSchedule, schedule =>
        {
            schedule.HasCheckConstraint("CK_WorkoutEntries_WorkoutSchedule_DurationCheck", "WorkoutSchedule_Duration BETWEEN '00:10:00' AND '05:00:00'");
            
            schedule.Property(s => s.Start)
                .IsRequired();
            
            schedule.Property(s => s.End)
                .IsRequired();
            
            schedule.Property(s => s.Duration)
                .IsRequired();
        });
        
        // Indexes
        builder.HasIndex(we => we.WorkoutProgramId);
        
        // Navigation properties
        builder.HasOne(we => we.WorkoutProgram)
            .WithMany()
            .HasForeignKey(we => we.WorkoutProgramId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}