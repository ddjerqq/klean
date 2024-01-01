using Application.Common.Interfaces;
using Domain.ValueObjects;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Inventory.Commands;


/// <summary>
/// Command for creating an item type.
/// </summary>
public sealed record CreateItemTypeCommand(string Id, string Name, decimal Price, float MinRarity, float MaxRarity)
    : IRequest<ItemType?>
{
    /// <summary>
    /// Converts this command to an item type.
    /// </summary>
    public ItemType ItemType => new(Id, Name, Price, MinRarity, MaxRarity);
}

internal sealed class CreateItemTypeValidator : AbstractValidator<CreateItemTypeCommand>
{
    public CreateItemTypeValidator(IAppDbContext dbContext)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .Matches(@"^[A-Z_]{3,50}$")
            .WithMessage("Id must be uppercase and contain only letters and underscores, 3-50 characters long");

        RuleFor(x => x.Id)
            .MustAsync(async (_, id, ct) =>
            {
                var exists = await dbContext.Set<ItemType>().AnyAsync(x => x.Id == id, ct);
                return !exists;
            })
            .WithMessage("An item type with this Id already exists.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(50);

        RuleFor(x => x.Price)
            .GreaterThan(0);

        RuleFor(x => x.MinRarity)
            .GreaterThan(0)
            .LessThanOrEqualTo(x => x.MaxRarity);

        RuleFor(x => x.MaxRarity)
            .GreaterThanOrEqualTo(x => x.MinRarity)
            .LessThan(1);
    }
}

internal sealed class CreateItemTypeCommandHandler(IAppDbContext dbContext)
    : IRequestHandler<CreateItemTypeCommand, ItemType?>
{
    public async Task<ItemType?> Handle(CreateItemTypeCommand command, CancellationToken ct)
    {
        var itemType = command.ItemType;

        await dbContext.Set<ItemType>().AddAsync(itemType, ct);
        await dbContext.SaveChangesAsync(ct);

        return itemType;
    }
}