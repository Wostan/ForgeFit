using HabitsDaily.Domain.Aggregates.ShopAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HabitsDaily.Infrastructure.Persistence.Configurations;

public class ShopItemConfiguration : IEntityTypeConfiguration<ShopItem>
{
    public void Configure(EntityTypeBuilder<ShopItem> builder)
    {
        builder.HasKey(s => s.Id);
        
        //Properties
        builder.Property(s => s.Name)
            .HasMaxLength(200)
            .IsRequired();
        
        builder.Property(s => s.Description)
            .HasMaxLength(1000);
        
        builder.Property(s => s.Price)
            .IsRequired();
        
        builder.Property(s => s.Type)
            .HasConversion<int>()
            .IsRequired();
        
        builder.Property(s => s.CreatedAt)
            .IsRequired();
        
        //Indexes
        builder.HasIndex(s => s.Name).IsUnique();
        
        //Navigation properties TODO
    }
}