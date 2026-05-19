using ForgeFit.Domain.Aggregates.FoodAggregate;
using ForgeFit.Domain.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForgeFit.Infrastructure.Persistence.Configurations;

public class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> builder)
    {
        builder.ToTable("Recipes");

        builder.HasKey(r => r.Id);

        // Properties
        builder.Property(r => r.UserId)
            .IsRequired();

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(DomainConstants.ValidationLimits.MaxFoodLabelLength);

        builder.Property(r => r.Description)
            .HasMaxLength(DomainConstants.ValidationLimits.MaxDescriptionLength);

        builder.Property(r => r.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();

        builder.Property(r => r.UpdatedAt);

        builder.OwnsMany(r => r.Ingredients, item =>
        {
            item.ToTable("RecipeIngredients", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint("CK_RecipeIngredients_CaloriesCheck", "Calories >= 0");
                tableBuilder.HasCheckConstraint("CK_RecipeIngredients_CarbsCheck", "Carbs >= 0");
                tableBuilder.HasCheckConstraint("CK_RecipeIngredients_ProteinCheck", "Protein >= 0");
                tableBuilder.HasCheckConstraint("CK_RecipeIngredients_FatCheck", "Fat >= 0");
                tableBuilder.HasCheckConstraint("CK_RecipeIngredients_FiberCheck", "Fiber >= 0");
                tableBuilder.HasCheckConstraint("CK_RecipeIngredients_SugarCheck", "Sugar >= 0");
                tableBuilder.HasCheckConstraint("CK_RecipeIngredients_SaturatedFatCheck", "SaturatedFat >= 0");
                tableBuilder.HasCheckConstraint("CK_RecipeIngredients_SodiumCheck", "Sodium >= 0");
                tableBuilder.HasCheckConstraint("CK_RecipeIngredients_AmountCheck",
                    $"Amount >= {DomainConstants.ValidationLimits.MinFoodAmount} AND Amount <= {DomainConstants.ValidationLimits.MaxFoodAmount}");
            });

            item.WithOwner()
                .HasForeignKey("RecipeId");

            item.Property(i => i.ExternalId)
                .IsRequired()
                .HasMaxLength(DomainConstants.ValidationLimits.MaxExternalIdLength);

            item.Property(i => i.Label)
                .IsRequired()
                .HasMaxLength(DomainConstants.ValidationLimits.MaxFoodLabelLength);

            item.Property(i => i.Calories)
                .IsRequired();

            item.Property(i => i.Carbs)
                .IsRequired();

            item.Property(i => i.Protein)
                .IsRequired();

            item.Property(i => i.Fat)
                .IsRequired();

            item.Property(i => i.Fiber)
                .IsRequired();

            item.Property(i => i.Sugar)
                .IsRequired();

            item.Property(i => i.SaturatedFat)
                .IsRequired();

            item.Property(i => i.Sodium)
                .IsRequired();

            item.Property(i => i.ServingUnit)
                .IsRequired()
                .HasDefaultValue("g");

            item.Property(i => i.Amount)
                .IsRequired()
                .HasDefaultValue(100);

            item.Property<int>("Id");
            item.HasKey("Id");

            item.HasIndex("RecipeId");
        });

        // Indexes
        builder.HasIndex(r => r.UserId);
        
        // Navigation properties
        builder.HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
