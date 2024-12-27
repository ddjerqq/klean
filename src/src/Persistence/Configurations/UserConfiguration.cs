using System.Globalization;
using Domain.Aggregates;
using EntityFrameworkCore.DataProtection.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(x => x.PersonalId)
            .IsEncryptedQueryable(isUnique: true)
            .HasMaxLength(12);

        builder.HasIndex(x => x.PersonalId)
            .IsUnique();

        builder.Property(x => x.Username)
            .IsEncryptedQueryable(isUnique: false)
            .HasMaxLength(100);

        builder.Property(x => x.Email)
            .IsEncryptedQueryable(isUnique: true)
            .HasMaxLength(100);

        builder.Property(x => x.PhoneNumber)
            .IsEncryptedQueryable(isUnique: true)
            .HasMaxLength(20);

        // email confirmed
        // phone confirmed

        builder.Property(x => x.PasswordHash).HasMaxLength(100);
        builder.Property(x => x.SecurityStamp).HasMaxLength(36); // guid
        builder.Property(x => x.ConcurrencyStamp).IsConcurrencyToken().HasMaxLength(36); // guid

        // lockout end
        // access failed count

        builder.Property(x => x.CultureInfo)
            .HasConversion<string>(
                culture => culture.ToString(),
                str => CultureInfo.GetCultureInfo(str));

        builder.Property(x => x.TimeZone)
            .HasConversion<string>(
                tz => tz.Id,
                id => TimeZoneInfo.FindSystemTimeZoneById(id));

        builder.HasMany(user => user.Claims)
            .WithOne(uc => uc.User)
            .HasForeignKey(uc => uc.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasMany(user => user.Logins)
            .WithOne(ul => ul.User)
            .HasForeignKey(ul => ul.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasMany(user => user.Roles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }

}