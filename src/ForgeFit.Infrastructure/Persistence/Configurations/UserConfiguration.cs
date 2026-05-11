using ForgeFit.Domain.Aggregates.UserAggregate;
using ForgeFit.Domain.Constants;
using ForgeFit.Domain.Enums.ProfileEnums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForgeFit.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", tableBuilder =>
        {
            tableBuilder.HasCheckConstraint("CK_Users_UserProfile_GenderCheck", 
                $"UserProfile_Gender IN ({string.Join(", ", Enum.GetValues<Gender>().Cast<int>())})");
            tableBuilder.HasCheckConstraint("CK_Users_WeightCheck", 
                $"(WeightUnit = {(int)WeightUnit.Kg} AND WeightValue >= {DomainConstants.ValidationLimits.MinWeightKg} AND WeightValue <= {DomainConstants.ValidationLimits.MaxWeightKg}) OR " +
                $"(WeightUnit = {(int)WeightUnit.Lb} AND WeightValue >= {DomainConstants.ValidationLimits.MinWeightLbs} AND WeightValue <= {DomainConstants.ValidationLimits.MaxWeightLbs})");
            tableBuilder.HasCheckConstraint("CK_Users_WeightUnitCheck", 
                $"WeightUnit IN ({string.Join(", ", Enum.GetValues<WeightUnit>().Cast<int>())})");
            tableBuilder.HasCheckConstraint("CK_Users_HeightCheck", 
                $"(HeightUnit = {(int)HeightUnit.Cm} AND HeightValue >= {DomainConstants.ValidationLimits.MinHeightCm} AND HeightValue <= {DomainConstants.ValidationLimits.MaxHeightCm}) OR " +
                $"(HeightUnit = {(int)HeightUnit.Inch} AND HeightValue >= {DomainConstants.ValidationLimits.MinHeightInches} AND HeightValue <= {DomainConstants.ValidationLimits.MaxHeightInches})");
            tableBuilder.HasCheckConstraint("CK_Users_HeightUnitCheck", 
                $"HeightUnit IN ({string.Join(", ", Enum.GetValues<HeightUnit>().Cast<int>())})");
        });

        builder.HasKey(u => u.Id);

        // Properties
        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(DomainConstants.ValidationLimits.MaxPasswordHashLength);

        builder.Property(u => u.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();

        builder.Property(u => u.UpdatedAt)
            .IsRequired(false);

        // ValueObject properties
        builder.OwnsOne(u => u.Email, email =>
        {
            email.Property(e => e.Value)
                .IsRequired()
                .HasMaxLength(DomainConstants.ValidationLimits.MaxEmailLength);

            email.HasIndex(e => e.Value)
                .IsUnique();
        });

        builder.OwnsOne(u => u.UserProfile, profile =>
        {
            profile.Property(p => p.Username)
                .IsRequired()
                .HasMaxLength(DomainConstants.ValidationLimits.MaxUsernameLength);

            profile.Property(p => p.AvatarUrl)
                .HasMaxLength(DomainConstants.ValidationLimits.MaxAvatarUrlLength)
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
                weight.Property(w => w.Value)
                    .IsRequired()
                    .HasColumnName("WeightValue");

                weight.Property(w => w.Unit)
                    .IsRequired()
                    .HasColumnName("WeightUnit");
            });

            profile.OwnsOne(p => p.Height, height =>
            {
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
