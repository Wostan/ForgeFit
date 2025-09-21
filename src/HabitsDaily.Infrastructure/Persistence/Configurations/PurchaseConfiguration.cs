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
            .IsRequired();
        
        builder.Property(p => p.CreatedAt)
            .IsRequired();
        
        //Indexes
        builder.HasIndex(p => new { p.UserId, p.ShopItemId })
            .IsUnique()
            .HasFilter("[ShopItemId] IN (SELECT Id FROM ShopItems WHERE Type IN (1,2))");
        
        //Navigation properties TODO
    }
}