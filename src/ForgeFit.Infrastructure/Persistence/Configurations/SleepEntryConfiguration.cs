using ForgeFit.Domain.Aggregates.SleepAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForgeFit.Infrastructure.Persistence.Configurations;

public class SleepEntryConfiguration : IEntityTypeConfiguration<SleepEntry>
{
    [Obsolete("Obsolete")]
    public void Configure(EntityTypeBuilder<SleepEntry> builder)
    {
        builder.ToTable("SleepEntries");
        
        builder.HasKey(se => se.Id);
        
        // Properties
        builder.Property(se => se.SleepDate)
            .IsRequired();
        
        // ValueObject properties
        builder.OwnsOne(se => se.SleepSchedule, schedule =>
        {
            schedule.HasCheckConstraint("CK_SleepEntries_SleepSchedule_DurationCheck", "SleepSchedule_Duration BETWEEN '01:00:00' AND '14:00:00'");
            
            schedule.Property(s => s.Start)
                .HasColumnName("SleepSchedule_Bedtime")
                .IsRequired();
            
            schedule.Property(s => s.End)
                .HasColumnName("SleepSchedule_Waketime")
                .IsRequired();
            
            schedule.Property(s => s.Duration)
                .HasColumnName("SleepSchedule_Duration")
                .IsRequired();
        });
        
        // Indexes
        builder.HasIndex(se => se.UserId);
        
        // Navigation properties
        builder.HasOne(se => se.User)
            .WithMany()
            .HasForeignKey(se => se.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}