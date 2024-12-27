namespace Domain.ValueObjects;

public readonly record struct Money
{
    public Currency Currency { get; }
    public decimal Amount { get; }

    public Money(Currency currency, decimal amount)
    {
        if (Amount < 0)
            throw new InvalidOperationException("Amount cannot be negative.");

        Currency = currency;
        Amount = amount;
    }

    public Money(string currency, decimal amount) : this(new Currency(currency), amount) { }

    public static Money Parse(string value)
    {
        var parts = value.Split('-');

        return parts switch
        {
            [var currency, var amount] => new Money(new Currency(currency), decimal.Parse(parts[1])),
            _ => throw new FormatException("Invalid money format."),
        };
    }

    public override string ToString() => $"{Currency}-{Amount}";
}