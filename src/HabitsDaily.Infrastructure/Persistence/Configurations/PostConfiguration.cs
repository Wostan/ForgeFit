using HabitsDaily.Domain.Aggregates.PostAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HabitsDaily.Infrastructure.Persistence.Configurations;

public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.HasKey(p => p.Id);
        
        //Properties
        builder.Property(p => p.UserId)
            .IsRequired();

        builder.Property(p => p.TextContent)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(p => p.MediaUrl)
            .HasConversion(
                v => v != null ? v.ToString() : null,
                v => v != null ? new Uri(v) : null
            );

        builder.Property(p => p.ViewsCount)
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .IsRequired();
        
        //Indexes
        builder.HasIndex(p => p.UserId);
        
        //Navigation properties
        builder.HasOne(p => p.User)
            .WithMany(u => u.Posts)
            .HasForeignKey(p => p.UserId);
        
        builder.HasMany(p => p.Comments)
            .WithOne(c => c.Post)
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(p => p.Likes)
            .WithOne(l => l.Post)
            .HasForeignKey(l => l.PostId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}