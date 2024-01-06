using System.ComponentModel;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

[EditorBrowsable(EditorBrowsableState.Never)]
internal class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        // Inventory is Dictionary<Guid, Item>
        // Inventory.Items is IEnumerable<Item>
        builder.HasOne(e => e.Owner)
            .WithMany(x => x.Inventory.Items);
    }
}