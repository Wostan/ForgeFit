using ForgeFit.Domain.Aggregates.FoodAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForgeFit.Infrastructure.Persistence.Configurations;

public class FoodEntryConfiguration : IEntityTypeConfiguration<FoodEntry>
{
    public void Configure(EntityTypeBuilder<FoodEntry> builder)
    {
        builder.ToTable("FoodEntries", tableBuilder =>
        {
            tableBuilder.HasCheckConstraint("CK_FoodEntries_CaloriesCheck", "Calories > 0");
            tableBuilder.HasCheckConstraint("CK_FoodEntries_CarbsCheck", "Carbs > 0");
            tableBuilder.HasCheckConstraint("CK_FoodEntries_ProteinCheck", "Protein > 0");
            tableBuilder.HasCheckConstraint("CK_FoodEntries_FatCheck", "Fat > 0");
            tableBuilder.HasCheckConstraint("CK_FoodEntries_DayTimeCheck", "DayTime IN (1, 2, 3, 4)");
        });

        builder.HasKey(fe => fe.Id);

        // Properties
        builder.Property(fe => fe.Calories)
            .IsRequired();

        builder.Property(fe => fe.Carbs)
            .IsRequired();

        builder.Property(fe => fe.Protein)
            .IsRequired();

        builder.Property(fe => fe.Fat)
            .IsRequired();

        builder.Property(fe => fe.DayTime)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(fe => fe.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();

        // ValueObject properties
        builder.OwnsMany(fe => fe.FoodItems, item =>
        {
            item.ToTable("FoodItems", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint("CK_FoodItems_CaloriesCheck", "Calories > 0");
                tableBuilder.HasCheckConstraint("CK_FoodItems_CarbsCheck", "Carbs > 0");
                tableBuilder.HasCheckConstraint("CK_FoodItems_ProteinCheck", "Protein > 0");
                tableBuilder.HasCheckConstraint("CK_FoodItems_FatCheck", "Fat > 0");
                tableBuilder.HasCheckConstraint("CK_FoodItems_AmountCheck", "Amount > 0");
            });

            item.WithOwner()
                .HasForeignKey("FoodEntryId");

            item.Property(i => i.ExternalId)
                .IsRequired()
                .HasMaxLength(100);

            item.Property(i => i.Label)
                .IsRequired()
                .HasMaxLength(100);

            item.Property(i => i.Calories)
                .IsRequired();

            item.Property(i => i.Carbs)
                .IsRequired();

            item.Property(i => i.Protein)
                .IsRequired();

            item.Property(i => i.Fat)
                .IsRequired();

            item.Property(i => i.ServingUnit)
                .IsRequired()
                .HasDefaultValue("g");

            item.Property(i => i.Amount)
                .IsRequired()
                .HasDefaultValue(100);

            item.Property<int>("Id");
            item.HasKey("Id");

            item.HasIndex("FoodEntryId");
        });

        // Indexes
        builder.HasIndex(fe => fe.UserId);

        // Navigation properties
        builder.HasOne(fe => fe.User)
            .WithMany()
            .HasForeignKey(fe => fe.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
