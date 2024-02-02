using Application.Abstractions;

namespace Infrastructure.Services;

/// <inheritdoc />
public sealed class UtcDateTimeProvider : IDateTimeProvider
{
    /// <inheritdoc />
    public DateTime UtcNow => DateTime.UtcNow;
}