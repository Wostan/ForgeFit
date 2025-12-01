using ForgeFit.Domain.Aggregates.NotificationAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForgeFit.Infrastructure.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications", tableBuilder =>
        {
            tableBuilder.HasCheckConstraint("CK_Notifications_NotificationTypeCheck",
                "NotificationType IN (1, 2, 3, 4, 5)");
            tableBuilder.HasCheckConstraint("CK_Notifications_Frequency_FrequencyUnitCheck",
                "Frequency_FrequencyUnit IN (1, 2, 3)");
            tableBuilder.HasCheckConstraint("CK_Notifications_Frequency_IntervalCheck", "Frequency_Interval > 0");
        });

        builder.HasKey(n => n.Id);

        // Properties
        builder.Property(n => n.NotificationType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(n => n.Title)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(n => n.Body)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(n => n.ScheduledAt)
            .IsRequired();

        builder.Property(n => n.IsSent)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(n => n.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();

        // ValueObject properties
        builder.OwnsOne(n => n.Frequency, freq =>
        {
            freq.Property(f => f.FrequencyUnit)
                .HasConversion<int>()
                .IsRequired();

            freq.Property(f => f.Interval)
                .IsRequired();
        });

        // Navigation properties
        builder.HasOne(n => n.User)
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
