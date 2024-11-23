using Application.Services;
using Domain.Aggregates;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Cqrs.Users.Queries;

public sealed record GetAllUsersQuery(int Page, int PerPage) : IRequest<IEnumerable<User>>;

internal sealed class GetAllUsersQueryHandler(IAppDbContext dbContext) : IRequestHandler<GetAllUsersQuery, IEnumerable<User>>
{
    public async Task<IEnumerable<User>> Handle(GetAllUsersQuery request, CancellationToken ct)
    {
        return await dbContext.Users
            .OrderBy(x => x.Id)
            .Skip(request.Page * request.PerPage)
            .Take(request.PerPage)
            .ToListAsync(ct);
    }
}