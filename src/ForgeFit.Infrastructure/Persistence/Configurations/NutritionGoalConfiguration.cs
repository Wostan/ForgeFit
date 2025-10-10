using ForgeFit.Domain.Aggregates.GoalAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForgeFit.Infrastructure.Persistence.Configurations;

public class NutritionGoalConfiguration : IEntityTypeConfiguration<NutritionGoal>
{
    [Obsolete("Obsolete")]
    public void Configure(EntityTypeBuilder<NutritionGoal> builder)
    {
        builder.ToTable("NutritionGoals")
            .HasCheckConstraint("CK_NutritionGoals_CaloriesCheck", "Calories > 0")
            .HasCheckConstraint("CK_NutritionGoals_CarbsCheck", "Carbs > 0")
            .HasCheckConstraint("CK_NutritionGoals_ProteinCheck", "Protein > 0")
            .HasCheckConstraint("CK_NutritionGoals_FatCheck", "Fat > 0")
            .HasCheckConstraint("CK_NutritionGoals_WaterGoalMlCheck", "WaterGoalMl > 0");
        
        builder.HasKey(n => n.Id);
        
        // Properties
        builder.Property(n => n.Calories)
            .IsRequired();

        builder.Property(n => n.Carbs)
            .IsRequired();
        
        builder.Property(n => n.Protein)
            .IsRequired();
        
        builder.Property(n => n.Fat)
            .IsRequired();
        
        builder.Property(n => n.WaterGoalMl)
            .IsRequired();
        
        builder.Property(n => n.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();
        
        // Indexes
        builder.HasIndex(n => n.UserId);
        
        // Navigation properties
        builder.HasOne(n => n.User)
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}