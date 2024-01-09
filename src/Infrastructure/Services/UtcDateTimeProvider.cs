using Application.Common.Interfaces;

namespace Infrastructure.Services;

/// <inheritdoc />
public sealed class UtcDateTimeProvider : IDateTimeProvider
{
    /// <inheritdoc />
    public DateTime UtcNow => DateTime.UtcNow;
}