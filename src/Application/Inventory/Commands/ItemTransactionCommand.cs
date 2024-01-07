using Application.Common.Extensions;
using Application.Common.Interfaces;
using Domain.Aggregates;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.Inventory.Commands;

/// <summary>
/// The base item transaction command.
/// </summary>
public sealed record ItemTransactionCommand(User Sender, User Receiver, Item Item) : IRequest<bool>;

internal sealed class ItemTransactionValidator : AbstractValidator<ItemTransactionCommand>
{
    public ItemTransactionValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Sender)
            .Must((cmd, sender) => sender.Inventory.HasItemWithId(cmd.Item.Id))
            .WithMessage("Sender does not have this item.");
    }
}

internal sealed class ItemTransactionHandler(IAppDbContext dbContext)
    : IRequestHandler<ItemTransactionCommand, bool>
{
    public async Task<bool> Handle(ItemTransactionCommand command, CancellationToken ct)
    {
        if (!command.Sender.Inventory.HasItemWithId(command.Item.Id))
            return false;

        if (!command.Sender.Inventory.TryTransfer(command.Receiver.Inventory, command.Item))
            return false;

        dbContext.Set<User>().TryUpdateIfNotNull(command.Sender);
        dbContext.Set<User>().TryUpdateIfNotNull(command.Receiver);
        await dbContext.SaveChangesAsync(ct);

        return true;
    }
}