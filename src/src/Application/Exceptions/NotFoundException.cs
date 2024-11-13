using System.Diagnostics.CodeAnalysis;

namespace Application.Exceptions;

/// <summary>
///     Exception thrown when an entity is not found
/// </summary>
public sealed class NotFoundException(string message) : Exception(message)
{
    public static void ThrowIfNull([NotNull] object? obj, string? message = default)
    {
        if (obj is null)
            throw new NotFoundException(message ?? "Entity not found");
    }
}