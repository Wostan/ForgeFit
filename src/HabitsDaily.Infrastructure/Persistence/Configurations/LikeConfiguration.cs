using HabitsDaily.Domain.Aggregates.PostAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HabitsDaily.Infrastructure.Persistence.Configurations;

public class LikeConfiguration : IEntityTypeConfiguration<Like>
{
    public void Configure(EntityTypeBuilder<Like> builder)
    {
        builder.HasKey(l => l.Id);
        
        //Properties
        builder.Property(l => l.PostId)
            .IsRequired();
        
        builder.Property(l => l.UserId)
            .IsRequired();
        
        builder.Property(l => l.CreatedAt)
            .IsRequired();
        
        //Indexes
        builder.HasIndex(l => new { l.PostId, l.UserId }).IsUnique();
        
        //Navigation properties
        builder.HasOne(l => l.Post)
            .WithMany(p => p.Likes)
            .HasForeignKey(l => l.PostId);
        
        builder.HasOne(l => l.User)
            .WithMany(u => u.Likes)
            .HasForeignKey(l => l.UserId);
    }
}