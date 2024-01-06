using Domain.Common.Interfaces;

namespace Domain.ValueObjects;

/// <summary>
/// Represents a wallet of currency.
/// </summary>
/// <param name="Balance">The balance of the wallet</param>
public sealed record Wallet(decimal Balance = default) : IValueObject
{
    /// <summary>
    /// Gets the balance of the wallet.
    /// </summary>
    public decimal Balance { get; private set; } = Balance;

    /// <summary>
    /// Check if the wallet has a balance greater than or equal to the given amount.
    /// </summary>
    /// <param name="amount">The amount to check</param>
    /// <returns>True if the wallet has a balance greater than or equal to the given amount, false otherwise</returns>
    public bool HasBalance(decimal amount)
    {
        return Balance >= amount;
    }

    /// <summary>
    /// Try to transfer currency from this wallet to another.
    /// This operation is atomic.
    /// </summary>
    /// <param name="other">The wallet to transfer to</param>
    /// <param name="amount">The amount to transfer</param>
    /// <returns>True if the transfer was successful, false otherwise</returns>
    public bool TryTransfer(Wallet other, decimal amount)
    {
        if (!HasBalance(amount))
            return false;

        Balance -= amount;
        other.Balance += amount;

        return true;
    }
}