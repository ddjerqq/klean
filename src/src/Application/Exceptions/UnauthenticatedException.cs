using System.Diagnostics.CodeAnalysis;
using Domain.Aggregates;

namespace Application.Exceptions;

/// <summary>
///     Exception thrown when the user is not authenticated
/// </summary>
public sealed class UnauthenticatedException(string message) : Exception(message)
{
    public static void ThrowIfNull([NotNull] User? user, string? message = default)
    {
        if (user is null)
            throw new UnauthenticatedException(message ?? "User not authenticated");
    }
}