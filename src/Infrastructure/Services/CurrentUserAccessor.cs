using System.Security.Claims;
using Application.Common.Interfaces;
using Domain.Aggregates;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

/// <inheritdoc />
public sealed class CurrentUserAccessor(IHttpContextAccessor httpContextAccessor, IAppDbContext dbContext)
    : ICurrentUserAccessor
{
    /// <inheritdoc />
    public Guid? CurrentUserId
    {
        get
        {
            var claimsPrincipal = httpContextAccessor.HttpContext?.User;
            var stringId = claimsPrincipal?
                .Claims
                .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?
                .Value;

            return Guid.TryParse(stringId, out var id) ? id : null;
        }
    }

    /// <inheritdoc />
    public async Task<User?> GetCurrentUserAsync(CancellationToken ct = default)
    {
        var id = CurrentUserId;

        if (id is null)
            return null;

        return await dbContext.Set<User>().FirstOrDefaultAsync(u => u.Id == id, ct);
    }
}