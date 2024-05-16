using System.ComponentModel;
using Domain.Aggregates;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

[EditorBrowsable(EditorBrowsableState.Never)]
internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(e => e.Id)
            .HasConversion(
                id => id.Value,
                value => new UserId(value));

        builder.HasIndex(e => e.Username)
            .IsUnique();

        builder.Property(e => e.Wallet)
            .HasConversion(
                wallet => wallet.Balance,
                balance => new Wallet(balance));
    }
}