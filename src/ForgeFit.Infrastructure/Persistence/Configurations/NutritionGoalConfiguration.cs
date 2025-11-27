using ForgeFit.Domain.Aggregates.GoalAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForgeFit.Infrastructure.Persistence.Configurations;

public class NutritionGoalConfiguration : IEntityTypeConfiguration<NutritionGoal>
{
    public void Configure(EntityTypeBuilder<NutritionGoal> builder)
    {
        builder.ToTable("NutritionGoals", tableBuilder =>
        {
            tableBuilder.HasCheckConstraint("CK_NutritionGoals_DailyNutritionPlan_CaloriesCheck", 
                    "DailyNutritionPlan_TargetCalories > 1000");
            tableBuilder.HasCheckConstraint("CK_NutritionGoals_DailyNutritionPlan_CarbsCheck", 
                    "DailyNutritionPlan_Carbs > 0");
            tableBuilder.HasCheckConstraint("CK_NutritionGoals_DailyNutritionPlan_ProteinCheck", 
                    "DailyNutritionPlan_Protein > 0");
            tableBuilder.HasCheckConstraint("CK_NutritionGoals_DailyNutritionPlan_FatCheck", 
                    "DailyNutritionPlan_Fat > 0");
            tableBuilder.HasCheckConstraint("CK_NutritionGoals_DailyNutritionPlan_WaterGoalMlCheck", 
                    "DailyNutritionPlan_WaterMl > 1000");
        });
        
        builder.HasKey(n => n.Id);
        
        // Properties
        builder.Property(n => n.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();
        
        // ValueObject properties
        builder.OwnsOne(n => n.DailyNutritionPlan, dailyNutritionPlan =>
        {
            dailyNutritionPlan.Property(dnp => dnp.TargetCalories)
                .IsRequired();
            
            dailyNutritionPlan.Property(dnp => dnp.Protein)
                .IsRequired();
            
            dailyNutritionPlan.Property(dnp => dnp.Carbs)
                .IsRequired();
            
            dailyNutritionPlan.Property(dnp => dnp.Fat)
                .IsRequired();
            
            dailyNutritionPlan.Property(dnp => dnp.WaterMl)
                .IsRequired();
        });
        
        // Indexes
        builder.HasIndex(n => n.UserId);
        
        // Navigation properties
        builder.HasOne(n => n.User)
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}