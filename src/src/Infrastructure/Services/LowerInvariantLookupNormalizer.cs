using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services;

public sealed class LowerInvariantLookupNormalizer : ILookupNormalizer
{
    [return: NotNullIfNotNull("name")]
    public string? NormalizeName(string? name) =>
        name?.Normalize().ToLowerInvariant();

    [return: NotNullIfNotNull("email")]
    public string? NormalizeEmail(string? email) => email?.Normalize().ToLowerInvariant();
}