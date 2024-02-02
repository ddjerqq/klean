using System.ComponentModel;
using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Inventory.Commands;

/// <summary>
/// Command for buying an item.
/// </summary>
public sealed record BuyItemCommand(string ItemTypeId) : IRequest<Item?>;

[EditorBrowsable(EditorBrowsableState.Never)]
internal sealed class BuyItemValidator : AbstractValidator<BuyItemCommand>
{
    public BuyItemValidator(IAppDbContext dbContext, ICurrentUserAccessor currentUserAccessor)
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.ItemTypeId)
            .MustAsync(async (_, itemType, ct) =>
            {
                var type = await dbContext.Set<ItemType>().FirstOrDefaultAsync(x => x.Id == itemType, ct);
                var user = await currentUserAccessor.GetCurrentUserAsync(ct);

                return type is { Price: var price }
                       && user is { Wallet: var wallet }
                       && wallet.HasBalance(price);
            })
            .WithMessage("Either the ItemType does not exist, or you do not have enough money to buy an item of that type.");
    }
}

[EditorBrowsable(EditorBrowsableState.Never)]
internal sealed class BuyItemHandler(IAppDbContext dbContext, ICurrentUserAccessor currentUserAccessor)
    : IRequestHandler<BuyItemCommand, Item?>
{
    public async Task<Item?> Handle(BuyItemCommand command, CancellationToken ct)
    {
        var type = await dbContext.Set<ItemType>().FirstOrDefaultAsync(x => x.Id == command.ItemTypeId, ct);
        var user = await currentUserAccessor.GetCurrentUserAsync(ct);

        if (type is { Price: var price } && user is { Wallet: var wallet } && wallet.HasBalance(price))
        {
            var item = type.NewItem();
            item.Owner = user;
            return item;
        }

        return null;
    }
}