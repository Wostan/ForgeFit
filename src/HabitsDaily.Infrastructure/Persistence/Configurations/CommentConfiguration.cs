using HabitsDaily.Domain.Aggregates.PostAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HabitsDaily.Infrastructure.Persistence.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(c => c.Id);
        
        //Properties
        builder.Property(c => c.PostId)
            .IsRequired();
        
        builder.Property(c => c.UserId)
            .IsRequired();
        
        builder.Property(c => c.TextContent)
            .HasMaxLength(1000)
            .IsRequired();
        
        builder.Property(c => c.CreatedAt)
            .IsRequired();
        
        //Indexes
        builder.HasIndex(c => new { c.PostId, c.UserId });
        
        //Navigation properties
        builder.HasOne(c => c.Post)
            .WithMany(p => p.Comments)
            .HasForeignKey(c => c.PostId);
        
        builder.HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserId);
    }
}