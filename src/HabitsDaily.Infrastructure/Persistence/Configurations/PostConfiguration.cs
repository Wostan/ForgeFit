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
        
        //Navigation properties TODO
    }
}