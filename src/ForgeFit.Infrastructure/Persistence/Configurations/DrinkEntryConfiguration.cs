using ForgeFit.Domain.Aggregates.FoodAggregate;
using ForgeFit.Domain.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForgeFit.Infrastructure.Persistence.Configurations;

public class DrinkEntryConfiguration : IEntityTypeConfiguration<DrinkEntry>
{
    public void Configure(EntityTypeBuilder<DrinkEntry> builder)
    {
        builder.ToTable("DrinkEntries",
            tableBuilder => { 
                tableBuilder.HasCheckConstraint("CK_DrinkEntries_VolumeMlCheck", 
                    $"VolumeMl >= {DomainConstants.ValidationLimits.MinDrinkVolumeMl} AND VolumeMl <= {DomainConstants.ValidationLimits.MaxDrinkVolumeMl}"); 
            });

        builder.HasKey(de => de.Id);

        // Properties
        builder.Property(de => de.VolumeMl)
            .IsRequired();

        builder.Property(de => de.Date)
            .IsRequired();

        builder.Property(de => de.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();

        builder.Property(de => de.UpdatedAt)
            .IsRequired(false);

        // Indexes
        builder.HasIndex(de => de.UserId);

        // Navigation properties
        builder.HasOne(de => de.User)
            .WithMany()
            .HasForeignKey(de => de.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
