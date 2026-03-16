using ForgeFit.Domain.Aggregates.GoalAggregate;
using ForgeFit.Domain.Constants;
using ForgeFit.Domain.Enums.GoalEnums;
using ForgeFit.Domain.Enums.ProfileEnums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForgeFit.Infrastructure.Persistence.Configurations;

public class BodyGoalConfiguration : IEntityTypeConfiguration<BodyGoal>
{
    public void Configure(EntityTypeBuilder<BodyGoal> builder)
    {
        builder.ToTable("BodyGoals", tableBuilder =>
        {
            tableBuilder.HasCheckConstraint("CK_BodyGoals_GoalTypeCheck", $"GoalType IN ({string.Join(", ", Enum.GetValues<GoalType>().Cast<int>())})");
            tableBuilder.HasCheckConstraint("CK_BodyGoals_GoalStatusCheck", $"GoalStatus IN ({string.Join(", ", Enum.GetValues<GoalStatus>().Cast<int>())})");
            tableBuilder.HasCheckConstraint("CK_BodyGoals_WeightGoal_ValueCheck", 
                $"(WeightGoal_Unit = {(int)WeightUnit.Kg} AND WeightGoal_Value >= {DomainConstants.ValidationLimits.MinWeightKg} AND WeightGoal_Value <= {DomainConstants.ValidationLimits.MaxWeightKg}) OR " +
                $"(WeightGoal_Unit = {(int)WeightUnit.Lb} AND WeightGoal_Value >= {DomainConstants.ValidationLimits.MinWeightLbs} AND WeightGoal_Value <= {DomainConstants.ValidationLimits.MaxWeightLbs})");
            tableBuilder.HasCheckConstraint("CK_BodyGoals_WeightGoal_UnitCheck", $"WeightGoal_Unit IN ({string.Join(", ", Enum.GetValues<WeightUnit>().Cast<int>())})");
        });

        builder.HasKey(bg => bg.Id);

        // Properties
        builder.Property(bg => bg.Title)
            .IsRequired()
            .HasMaxLength(DomainConstants.ValidationLimits.MaxTitleLength);

        builder.Property(bg => bg.Description)
            .HasMaxLength(DomainConstants.ValidationLimits.MaxDescriptionLength);

        builder.Property(bg => bg.GoalType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(bg => bg.GoalStatus)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(bg => bg.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();

        builder.Property(bg => bg.UpdatedAt)
            .IsRequired(false);

        // ValueObject properties
        builder.OwnsOne(bg => bg.WeightGoal, weightGoal =>
        {
            weightGoal.Property(wg => wg.Value)
                .IsRequired();

            weightGoal.Property(wg => wg.Unit)
                .IsRequired();
        });

        // Indexes
        builder.HasIndex(bg => bg.UserId);

        // Navigation properties
        builder.HasOne(bg => bg.User)
            .WithMany()
            .HasForeignKey(bg => bg.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
