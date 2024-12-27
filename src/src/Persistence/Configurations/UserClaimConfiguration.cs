using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

internal class UserClaimConfiguration : IEntityTypeConfiguration<UserClaim>
{
    public void Configure(EntityTypeBuilder<UserClaim> builder)
    {
        builder.HasOne(uc => uc.User)
            .WithMany(user => user.Claims)
            .HasForeignKey(uc => uc.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.Property(x => x.ClaimType).HasMaxLength(64);
        builder.Property(x => x.ClaimValue).HasMaxLength(256);

        builder.Ignore(x => x.Claim);
    }
}

internal class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.HasKey(role => new { role.UserId, role.RoleId });

        builder.HasOne(role => role.User)
            .WithMany(user => user.Roles)
            .HasForeignKey(role => role.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasOne(role => role.Role)
            .WithMany()
            .HasForeignKey(role => role.RoleId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}