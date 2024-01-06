using System.ComponentModel;
using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

[EditorBrowsable(EditorBrowsableState.Never)]
internal class ItemTypeConfiguration : IEntityTypeConfiguration<ItemType>
{
    public void Configure(EntityTypeBuilder<ItemType> builder)
    {
        builder.HasIndex(e => e.Id)
            .IsUnique();

        builder.HasIndex(e => e.Name)
            .IsUnique();

        builder.HasMany<Item>()
            .WithOne(x => x.ItemType);

        SeedData(builder);
    }

    private static void SeedData(EntityTypeBuilder<ItemType> builder)
    {
        var data = new List<ItemType>
        {
            new("FISHING_ROD", "Fishing rod 🎣", 75m, 0.1f, 0.9f),
            new("HUNTING_RIFLE", "Hunting Rifle 🔫", 75m, 0.1f, 0.9f),
            new("SHOVEL", "Shovel 🪣", 75m, 0.1f, 0.9f),
            new("COMMON_FISH", "Common Fish 🐟", 5, 0.1f, 0.9f),
            new("RARE_FISH", "Rare Fish 🐡", 10, 0.1f, 0.9f),
            new("TROPICAL_FISH", "Tropical Fish 🐯", 20, 0.1f, 0.9f),
            new("SHARK", "Shark 🐠", 40, 0.1f, 0.9f),
            new("GOLDEN_FISH", "Golden Fish 🦈", 50, 0.1f, 0.9f),
            new("PIG", "Pig 🥇🐟", 5, 0.1f, 0.9f),
            new("DEER", "Deer 🐷", 10, 0.1f, 0.9f),
            new("BEAR", "Bear 🦌", 20, 0.1f, 0.9f),
            new("WOLF", "Wolf 🐺", 30, 0.1f, 0.9f),
            new("TIGER", "Tiger 🐻", 40, 0.1f, 0.9f),
            new("LION", "Lion 🦁", 50, 0.1f, 0.9f),
            new("ELEPHANT", "Elephant 🐯", 60, 0.1f, 0.9f),
            new("COPPER_COIN", "Copper Coin 🐘", 1, 0.1f, 0.9f),
            new("EMERALD", "Emerald 👛", 10, 0.1f, 0.9f),
            new("RUBY", "Ruby 🔶", 20, 0.1f, 0.9f),
            new("SAPPHIRE", "Sapphire 🔷", 30, 0.1f, 0.9f),
            new("AMETHYST", "Amethyst 🔴", 40, 0.1f, 0.9f),
            new("DIAMOND", "Diamond 💎", 50, 0.1f, 0.9f),
            new("KNIFE", "Knife 🔪", 50, 0.1f, 0.9f),
            new("WEDDING_RING", "Wedding Ring 💍", 1000, 0.01f, 0.9f),
        };

        builder.HasData(data);
    }
}