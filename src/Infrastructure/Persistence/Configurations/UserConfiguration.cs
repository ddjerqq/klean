using System.ComponentModel;
using Domain.Abstractions;
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
        builder.HasIndex(e => e.Username)
            .IsUnique();

        builder.Property(e => e.Wallet)
            .HasConversion(
                wallet => wallet.Balance,
                balance => new Wallet(balance));

        builder.Property(e => e.Inventory)
            .HasConversion(
                inventory => inventory.Items,
                items => (Inventory)items.ToDictionary(i => i.Id, i => i));

        builder.HasMany(e => e.Inventory.Items)
            .WithOne(x => x.Owner);
    }
}