using Application.Abstractions;
using Domain.Aggregates;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;

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
            var stringId = httpContextAccessor
                .HttpContext?
                .User
                .Claims
                .FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sid)?
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