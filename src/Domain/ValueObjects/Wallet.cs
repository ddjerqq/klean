using Domain.Common.Interfaces;

namespace Domain.ValueObjects;

public sealed record Wallet(decimal Balance = default) : IValueObject
{
    public decimal Balance { get; private set; } = Balance;

    public bool HasBalance(decimal amount)
    {
        return Balance >= amount;
    }

    public bool TryTransfer(Wallet other, decimal amount)
    {
        if (!HasBalance(amount))
            return false;

        Balance -= amount;
        other.Balance += amount;

        return true;
    }

    public static implicit operator decimal(Wallet wallet) => wallet.Balance;

    public static implicit operator Wallet(decimal balance) => new(balance);

    public bool Equals(IValueObject? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        if (other.GetType() != GetType()) return false;

        var otherWallet = (Wallet)other;
        return Balance == otherWallet.Balance;
    }
}