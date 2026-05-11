using ForgeFit.Domain.Aggregates.NotificationAggregate;
using ForgeFit.Domain.Constants;
using ForgeFit.Domain.Enums.HabitEnums;
using ForgeFit.Domain.Enums.NotificationEnums;
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
                $"NotificationType IN ({string.Join(", ", Enum.GetValues<NotificationType>().Cast<int>())})");
            tableBuilder.HasCheckConstraint("CK_Notifications_Frequency_FrequencyUnitCheck",
                $"Frequency_FrequencyUnit IN ({string.Join(", ", Enum.GetValues<FrequencyUnit>().Cast<int>())})");
            tableBuilder.HasCheckConstraint("CK_Notifications_Frequency_IntervalCheck", 
                "Frequency_Interval > 0");
        });

        builder.HasKey(n => n.Id);

        // Properties
        builder.Property(n => n.NotificationType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(n => n.Title)
            .IsRequired()
            .HasMaxLength(DomainConstants.ValidationLimits.MaxTitleLength);

        builder.Property(n => n.Body)
            .IsRequired()
            .HasMaxLength(DomainConstants.ValidationLimits.MaxDescriptionLength);

        builder.Property(n => n.ScheduledAt)
            .IsRequired();

        builder.Property(n => n.IsSent)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(n => n.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();

        builder.Property(n => n.UpdatedAt)
            .IsRequired(false);

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
