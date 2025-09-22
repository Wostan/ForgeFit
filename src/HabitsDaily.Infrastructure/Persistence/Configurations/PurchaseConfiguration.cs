using HabitsDaily.Domain.Aggregates.ShopAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HabitsDaily.Infrastructure.Persistence.Configurations;

public class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
{
    public void Configure(EntityTypeBuilder<Purchase> builder)
    {
        builder.HasKey(p => p.Id);
        
        //Properties
        builder.Property(p => p.UserId)
            .IsRequired();
        
        builder.Property(p => p.ShopItemId)
            .IsRequired();
        
        builder.Property(p => p.Quantity)
            .IsRequired();
        
        builder.Property(p => p.TotalPrice)
            .HasPrecision(18, 2)
            .IsRequired();
        
        builder.Property(p => p.CreatedAt)
            .IsRequired();
        
        //Indexes
        builder.HasIndex(p => new { p.UserId, p.ShopItemId });
        
        //Navigation properties
        builder.HasOne(p => p.User)
            .WithMany(u => u.Purchases)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(p => p.ShopItem)
            .WithMany(si => si.Purchases)
            .HasForeignKey(p => p.ShopItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}