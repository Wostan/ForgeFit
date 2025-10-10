using ForgeFit.Domain.Aggregates.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForgeFit.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    [Obsolete("Obsolete")]
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        
        builder.HasKey(u => u.Id);
        
        // Properties
        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(256);
        
        builder.Property(u => u.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();
        
        // ValueObject properties
        builder.OwnsOne(u => u.Email, email =>
        { 
            email.Property(e => e.Value)
                .IsRequired()
                .HasMaxLength(100);

            email.HasIndex(e => e.Value)
                .IsUnique();
        });
        
        builder.OwnsOne(u => u.UserProfile, profile =>
        {
            profile.HasCheckConstraint("CK_Users_UserProfile_GenderCheck", "UserProfile_Gender IN (1, 2, 3)");
            
            profile.Property(p => p.Username)
                .IsRequired()
                .HasMaxLength(20);

            profile.Property(p => p.AvatarUrl)
                .HasMaxLength(200)
                .HasConversion(
                    v => v != null ? v.ToString() : null,
                    v => v != null ? new Uri(v) : null
                );

            profile.OwnsOne(p => p.DateOfBirth, dob =>
            {
                dob.Property(d => d.Value)
                    .IsRequired();
            });

            profile.Property(p => p.Gender)
                .HasConversion<int>()
                .IsRequired();

            profile.OwnsOne(p => p.Weight, weight =>
            {
                weight.HasCheckConstraint("CK_Users_WeightCheck", "WeightValue > 0")
                    .HasCheckConstraint("CK_Users_WeightUnitCheck", "WeightUnit IN (1, 2)");
                
                weight.Property(w => w.Value)
                    .IsRequired()
                    .HasColumnName("WeightValue");

                weight.Property(w => w.Unit)
                    .IsRequired()
                    .HasColumnName("WeightUnit");
            });

            profile.OwnsOne(p => p.Height, height =>
            {
                height.HasCheckConstraint("CK_Users_HeightCheck", "HeightValue > 0")
                    .HasCheckConstraint("CK_Users_HeightUnitCheck", "HeightUnit IN (1, 2)");
                
                height.Property(h => h.Value)
                    .IsRequired()
                    .HasColumnName("HeightValue");

                height.Property(h => h.Unit)
                    .IsRequired()
                    .HasColumnName("HeightUnit");
            });
        });
    }
}