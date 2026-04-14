using ForgeFit.Domain.Aggregates.FoodAggregate;
using ForgeFit.Domain.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForgeFit.Infrastructure.Persistence.Configurations;

public class FoodProductConfiguration : IEntityTypeConfiguration<FoodProduct>
{
    public void Configure(EntityTypeBuilder<FoodProduct> builder)
    {
        builder.ToTable("FoodProducts", tableBuilder =>
        {
            tableBuilder.HasCheckConstraint("CK_FoodProducts_CaloriesCheck", "Calories >= 0");
            tableBuilder.HasCheckConstraint("CK_FoodProducts_CarbsCheck", "Carbs >= 0");
            tableBuilder.HasCheckConstraint("CK_FoodProducts_ProteinCheck", "Protein >= 0");
            tableBuilder.HasCheckConstraint("CK_FoodProducts_FatCheck", "Fat >= 0");
            tableBuilder.HasCheckConstraint("CK_FoodProducts_FiberCheck", "Fiber >= 0");
            tableBuilder.HasCheckConstraint("CK_FoodProducts_SugarCheck", "Sugar >= 0");
            tableBuilder.HasCheckConstraint("CK_FoodProducts_SaturatedFatCheck", "SaturatedFat >= 0");
            tableBuilder.HasCheckConstraint("CK_FoodProducts_SodiumCheck", "Sodium >= 0");
            tableBuilder.HasCheckConstraint("CK_FoodProducts_ServingSizeCheck", "ServingSize > 0");
        });

        builder.HasKey(fp => fp.Id);

        // Properties
        builder.Property(fp => fp.ExternalId)
            .HasMaxLength(DomainConstants.ValidationLimits.MaxExternalIdLength);

        builder.Property(fp => fp.UserId);

        builder.Property(fp => fp.Name)
            .IsRequired()
            .HasMaxLength(DomainConstants.ValidationLimits.MaxFoodLabelLength);

        builder.Property(fp => fp.Brand)
            .HasMaxLength(DomainConstants.ValidationLimits.MaxFoodLabelLength);

        builder.Property(fp => fp.Barcode)
            .HasMaxLength(100);

        builder.Property(fp => fp.Calories)
            .IsRequired();

        builder.Property(fp => fp.Carbs)
            .IsRequired();

        builder.Property(fp => fp.Protein)
            .IsRequired();

        builder.Property(fp => fp.Fat)
            .IsRequired();

        builder.Property(fp => fp.Fiber)
            .IsRequired();

        builder.Property(fp => fp.Sugar)
            .IsRequired();

        builder.Property(fp => fp.SaturatedFat)
            .IsRequired();

        builder.Property(fp => fp.Sodium)
            .IsRequired();

        builder.Property(fp => fp.ServingSize)
            .IsRequired();

        builder.Property(fp => fp.ServingUnit)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(fp => fp.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();

        builder.Property(fp => fp.UpdatedAt);

        // Indexes
        builder.HasIndex(fp => fp.ExternalId);
        builder.HasIndex(fp => fp.UserId);
        builder.HasIndex(fp => fp.Barcode);
        builder.HasIndex(fp => new { fp.ExternalId, fp.UserId })
            .IsUnique()
            .HasFilter("[ExternalId] IS NOT NULL AND [UserId] IS NOT NULL");
        
        // Navigation properties
        builder.HasOne(fp => fp.User)
            .WithMany()
            .HasForeignKey(fp => fp.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
