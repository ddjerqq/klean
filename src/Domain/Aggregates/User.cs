using Domain.Abstractions;
using Domain.Common.Interfaces;
using Domain.ValueObjects;

namespace Domain.Aggregates;

public readonly record struct UserId(Guid Value)
{
    public static UserId Empty => new(Guid.Empty);
    public static UserId NewUserId() => new(Guid.NewGuid());
}

public sealed class User(UserId id) : AggregateRootBase<UserId>(id)
{
    public string Username { get; init; } = default!;

    public string Email { get; init; } = default!;
    
    public string PasswordHash { get; private set; } = default!;

    public string? ProfilePictureUrl { get; set; }
    
    public Wallet Wallet { get; init; } = default!;

    public Inventory Inventory { get; init; } = default!;

    public void SetPassword(string newPassword)
    {
        PasswordHash = BC.HashPassword(newPassword);
    }
}