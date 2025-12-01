using ForgeFit.Domain.Aggregates.GoalAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForgeFit.Infrastructure.Persistence.Configurations;

public class BodyGoalConfiguration : IEntityTypeConfiguration<BodyGoal>
{
    public void Configure(EntityTypeBuilder<BodyGoal> builder)
    {
        builder.ToTable("BodyGoals", tableBuilder =>
        {
            tableBuilder.HasCheckConstraint("CK_BodyGoals_GoalTypeCheck", "GoalType IN (1, 2, 3, 4)");
            tableBuilder.HasCheckConstraint("CK_BodyGoals_GoalStatusCheck", "GoalStatus IN (1, 2, 3)");
            tableBuilder.HasCheckConstraint("CK_BodyGoals_WeightGoal_ValueCheck", "WeightGoal_Value > 0");
            tableBuilder.HasCheckConstraint("CK_BodyGoals_WeightGoal_UnitCheck", "WeightGoal_Unit IN (1, 2)");
        });

        builder.HasKey(bg => bg.Id);

        // Properties
        builder.Property(bg => bg.Title)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(bg => bg.Description)
            .HasMaxLength(100);

        builder.Property(bg => bg.GoalType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(bg => bg.GoalStatus)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(bg => bg.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();

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
