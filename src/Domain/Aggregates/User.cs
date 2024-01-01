using Domain.Common.Abstractions;
using Domain.ValueObjects;

namespace Domain.Aggregates;

/// <summary>
/// Represents a user in the app.
///
/// This is an aggregate root.
/// </summary>
public sealed class User : AggregateRootBase
{
    /// <summary>
    /// Gets or sets the user's username.
    /// </summary>
    public string Username { get; set; } = default!;

    /// <summary>
    /// Gets or sets the user's email.
    /// </summary>
    public string Email { get; set; } = default!;

    /// <summary>
    /// Gets or sets the user's username.
    /// </summary>
    public Wallet Wallet { get; set; } = default!;

    /// <summary>
    /// Gets or sets the user's inventory.
    /// </summary>
    public Inventory Inventory { get; set; } = default!;
}