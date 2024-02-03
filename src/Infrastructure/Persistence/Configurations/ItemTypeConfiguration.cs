using System.ComponentModel;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

[EditorBrowsable(EditorBrowsableState.Never)]
internal class ItemTypeConfiguration : IEntityTypeConfiguration<ItemType>
{
    public void Configure(EntityTypeBuilder<ItemType> builder)
    {
        builder.HasIndex(e => e.Name)
            .IsUnique();

        SeedData(builder);
    }

    private static void SeedData(EntityTypeBuilder<ItemType> builder)
    {
        var data = new List<ItemType>
        {
            new("FISHING_ROD", "Fishing rod 🎣", 75m, 0.1f, 0.9f, "https://api.qrserver.com/v1/create-qr-code/?qzone=1&data=FISHING_ROD"),
            new("HUNTING_RIFLE", "Hunting Rifle 🔫", 75m, 0.1f, 0.9f, "https://api.qrserver.com/v1/create-qr-code/?qzone=1&data=HUNTING_RIFLE"),
            new("SHOVEL", "Shovel 🪣", 75m, 0.1f, 0.9f, "https://api.qrserver.com/v1/create-qr-code/?qzone=1&data=SHOVEL"),
            new("COMMON_FISH", "Common Fish 🐟", 5, 0.1f, 0.9f, "https://api.qrserver.com/v1/create-qr-code/?qzone=1&data=COMMON_FISH"),
            new("RARE_FISH", "Rare Fish 🐡", 10, 0.1f, 0.9f, "https://api.qrserver.com/v1/create-qr-code/?qzone=1&data=RARE_FISH"),
            new("TROPICAL_FISH", "Tropical Fish 🐯", 20, 0.1f, 0.9f, "https://api.qrserver.com/v1/create-qr-code/?qzone=1&data=TROPICAL_FISH"),
            new("SHARK", "Shark 🐠", 40, 0.1f, 0.9f, "https://api.qrserver.com/v1/create-qr-code/?qzone=1&data=SHARK"),
            new("GOLDEN_FISH", "Golden Fish 🦈", 50, 0.1f, 0.9f, "https://api.qrserver.com/v1/create-qr-code/?qzone=1&data=GOLDEN_FISH"),
            new("PIG", "Pig 🥇🐟", 5, 0.1f, 0.9f, "https://api.qrserver.com/v1/create-qr-code/?qzone=1&data=PIG"),
            new("DEER", "Deer 🐷", 10, 0.1f, 0.9f, "https://api.qrserver.com/v1/create-qr-code/?qzone=1&data=DEER"),
            new("BEAR", "Bear 🦌", 20, 0.1f, 0.9f, "https://api.qrserver.com/v1/create-qr-code/?qzone=1&data=BEAR"),
            new("WOLF", "Wolf 🐺", 30, 0.1f, 0.9f, "https://api.qrserver.com/v1/create-qr-code/?qzone=1&data=WOLF"),
            new("TIGER", "Tiger 🐻", 40, 0.1f, 0.9f, "https://api.qrserver.com/v1/create-qr-code/?qzone=1&data=TIGER"),
            new("LION", "Lion 🦁", 50, 0.1f, 0.9f, "https://api.qrserver.com/v1/create-qr-code/?qzone=1&data=LION"),
            new("ELEPHANT", "Elephant 🐯", 60, 0.1f, 0.9f, "https://api.qrserver.com/v1/create-qr-code/?qzone=1&data=ELEPHANT"),
            new("COPPER_COIN", "Copper Coin 🐘", 1, 0.1f, 0.9f, "https://api.qrserver.com/v1/create-qr-code/?qzone=1&data=COPPER_COIN"),
            new("EMERALD", "Emerald 👛", 10, 0.1f, 0.9f, "https://api.qrserver.com/v1/create-qr-code/?qzone=1&data=EMERALD"),
            new("RUBY", "Ruby 🔶", 20, 0.1f, 0.9f, "https://api.qrserver.com/v1/create-qr-code/?qzone=1&data=RUBY"),
            new("SAPPHIRE", "Sapphire 🔷", 30, 0.1f, 0.9f, "https://api.qrserver.com/v1/create-qr-code/?qzone=1&data=SAPPHIRE"),
            new("AMETHYST", "Amethyst 🔴", 40, 0.1f, 0.9f, "https://api.qrserver.com/v1/create-qr-code/?qzone=1&data=AMETHYST"),
            new("DIAMOND", "Diamond 💎", 50, 0.1f, 0.9f, "https://api.qrserver.com/v1/create-qr-code/?qzone=1&data=DIAMOND"),
            new("KNIFE", "Knife 🔪", 50, 0.1f, 0.9f, "https://api.qrserver.com/v1/create-qr-code/?qzone=1&data=KNIFE"),
            new("WEDDING_RING", "Wedding Ring 💍", 1000, 0.01f, 0.9f, "https://api.qrserver.com/v1/create-qr-code/?qzone=1&data=WEDDING_RING"),
        };

        builder.HasData(data);
    }
}