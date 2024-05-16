using Application.Services;
using Application.Services.Interfaces;

namespace Infrastructure.Services;

public sealed class UtcDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}