using Application.Services;
using Domain.Aggregates;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Infrastructure.Services;

public sealed class CurrentUserAccessor(IHttpContextAccessor httpContextAccessor, IAppDbContext dbContext)
    : ICurrentUserAccessor
{
    public UserId? CurrentUserId
    {
        get
        {
            var stringId = httpContextAccessor
                .HttpContext?
                .User
                .Claims
                .FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sid)?
                .Value;

            return Ulid.TryParse(stringId, out var id)
                ? new UserId(id)
                : null;
        }
    }

    public async Task<User?> GetCurrentUserAsync(CancellationToken ct = default)
    {
        var id = CurrentUserId;

        if (id is null)
            return null;

        return await dbContext
            .Set<User>()
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, ct);
    }
}