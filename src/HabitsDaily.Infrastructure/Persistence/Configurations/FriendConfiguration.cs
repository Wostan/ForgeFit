using HabitsDaily.Domain.Aggregates.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HabitsDaily.Infrastructure.Persistence.Configurations;

public class FriendConfiguration : IEntityTypeConfiguration<Friend>
{
    public void Configure(EntityTypeBuilder<Friend> builder)
    {
        builder.HasKey(f => new { f.UserId, f.FriendId });
        
        //Properties
        
        builder.Property(f => f.UserId)
            .IsRequired();

        builder.Property(f => f.FriendId)
            .IsRequired();

        builder.Property(f => f.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(f => f.CreatedAt)
            .IsRequired();
        
        // Navigation properties
        builder.HasOne(f => f.User)
            .WithMany(u => u.Friends)
            .HasForeignKey(f => f.UserId);
        
        builder.HasOne(f => f.UserFriend)
            .WithMany(u => u.Friends)
            .HasForeignKey(f => f.FriendId);
    }
}