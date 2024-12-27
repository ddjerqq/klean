using Domain.Entities;
using EntityFrameworkCore.DataProtection.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

internal class UserLoginConfiguration : IEntityTypeConfiguration<UserLogin>
{
    public void Configure(EntityTypeBuilder<UserLogin> builder)
    {
        builder.HasOne(login => login.User)
            .WithMany(user => user.Logins)
            .HasForeignKey(login => login.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.Property(x => x.UserAgent).HasMaxLength(256).IsEncryptedQueryable(false);
        builder.Property(x => x.Location).HasMaxLength(64).IsEncrypted();
        builder.Property(x => x.IpAddress).HasMaxLength(64).IsEncrypted();

        builder.Ignore(x => x.DeviceType);
    }
}