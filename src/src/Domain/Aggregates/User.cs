using Domain.Abstractions;
using Domain.Common;
using Domain.Entities;
using Domain.ValueObjects;

namespace Domain.Aggregates;

public readonly record struct UserId(Ulid Value)
{
    public static UserId Empty => new(Ulid.Empty);
    public static UserId NewUserId() => new(Ulid.NewUlid());
    public override string ToString() => StrongIdHelper<UserId, Ulid>.Serialize(Value);
    public static UserId? TryParse(string? value) => StrongIdHelper<UserId, Ulid>.Deserialize(value, null);

    public static UserId Parse(string value) =>
        StrongIdHelper<UserId, Ulid>.Deserialize(value, null)
        ?? throw new FormatException("Input string was not in the correct format");
}

public sealed class User(UserId id) : AggregateRoot<UserId>(id)
{
    public string Username { get; init; } = default!;

    public Wallet Wallet { get; init; } = default!;

    public ICollection<Item> Inventory { get; init; } = default!;
}