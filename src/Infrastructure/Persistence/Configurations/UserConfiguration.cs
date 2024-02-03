using System.ComponentModel;
using Domain.Aggregates;
using Domain.Common.Extensions;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

[EditorBrowsable(EditorBrowsableState.Never)]
internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasIndex(e => e.Username)
            .IsUnique();

        builder.Property(e => e.Wallet)
            .HasConversion(
                wallet => wallet.Balance,
                balance => new Wallet(balance));

        if ("ASPNETCORE_ENVIRONMENT".FromEnv() == "Development")
            SeedData(builder);
    }

    private static void SeedData(EntityTypeBuilder<User> builder)
    {
        var user = new User
        {
            Username = "username",
            Email = "default@example.com",
            Wallet = new Wallet(100),
            Inventory = [],
        };
        user.SetPassword("password");

        builder.HasData(user);
    }
}