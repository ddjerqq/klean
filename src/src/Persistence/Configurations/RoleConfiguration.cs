using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

internal class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(64);
        builder.HasIndex(x => x.Name).IsUnique();

        builder.Property(x => x.ConcurrencyStamp).IsConcurrencyToken().HasMaxLength(36);

        builder.HasMany(role => role.Claims)
            .WithOne(rc => rc.Role)
            .HasForeignKey(rc => rc.RoleId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}