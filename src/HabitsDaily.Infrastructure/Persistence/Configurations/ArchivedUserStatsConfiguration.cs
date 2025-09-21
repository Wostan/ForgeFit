using HabitsDaily.Domain.Aggregates.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HabitsDaily.Infrastructure.Persistence.Configurations;

public class ArchivedUserStatsConfiguration : IEntityTypeConfiguration<ArchivedUserStats>
{
    public void Configure(EntityTypeBuilder<ArchivedUserStats> builder)
    {
        builder.HasKey(au => au.Id);
        
        //Properties
        builder.Property(au => au.UserId)
            .IsRequired();
        
        builder.Property(au => au.HabitId)
            .IsRequired();
        
        builder.Property(au => au.TotalPoints)
            .IsRequired();
        
        builder.Property(au => au.CreatedAt)
            .IsRequired();
        
        //Indexes
        builder.HasIndex(au => new {au.UserId, au.HabitId})
            .IsUnique();
        
        //Navigation properties TODO
    }
}