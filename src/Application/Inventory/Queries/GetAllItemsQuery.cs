using System.ComponentModel;
using Application.Abstractions;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Inventory.Queries;

/// <summary>
/// The query to get all items with pagination
/// </summary>
public sealed record GetAllItemsQuery(int Start, int Count)
    : IRequest<IEnumerable<Item>>;

[EditorBrowsable(EditorBrowsableState.Never)]
internal sealed class GetAllItemsValidator : AbstractValidator<GetAllItemsQuery>
{
    public GetAllItemsValidator()
    {
        RuleFor(x => x.Start)
            .GreaterThan(0);

        RuleFor(x => x.Count)
            .InclusiveBetween(10, 100);
    }
}

[EditorBrowsable(EditorBrowsableState.Never)]
internal sealed class GetAllItemsQueryHandler(IAppDbContext dbContext)
    : IRequestHandler<GetAllItemsQuery, IEnumerable<Item>>
{
    public async Task<IEnumerable<Item>> Handle(GetAllItemsQuery command, CancellationToken ct)
    {
        return await dbContext.Set<Item>()
            .Skip(command.Start)
            .Take(command.Count)
            .ToListAsync(ct);
    }
}