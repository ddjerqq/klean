using System.ComponentModel;
using Domain.Entities;
using Klean.Generated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

[EditorBrowsable(EditorBrowsableState.Never)]
internal class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.Property(e => e.Id)
            .HasConversion(
                id => id.ToString(),
                value => ItemId.Parse(value));
    }
}