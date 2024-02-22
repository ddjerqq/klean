using Application.Abstractions;
using Domain.Aggregates;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Infrastructure.Services;

public sealed class CurrentUserAccessor(IHttpContextAccessor httpContextAccessor, IAppDbContext dbContext)
    : ICurrentUserAccessor
{
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

    public async Task<User?> GetCurrentUserAsync(CancellationToken ct = default)
    {
        var currentUserId = CurrentUserId;

        if (currentUserId is null)
            return null;

        var id = new UserId(currentUserId.Value);

        return await dbContext.Set<User>().FirstOrDefaultAsync(u => u.Id == id, ct);
    }
}