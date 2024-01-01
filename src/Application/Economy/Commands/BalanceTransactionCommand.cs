using Application.Common.Extensions;
using Application.Common.Interfaces;
using Domain.Aggregates;
using FluentValidation;
using MediatR;

namespace Application.Economy.Commands;

/// <summary>
/// Base balance transaction command.
/// </summary>
/// <param name="Sender">The sender of the transaction.</param>
/// <param name="Receiver">The receiver of the transaction.</param>
/// <param name="Amount">The amount of the transaction.</param>
/// <returns>True if the transaction was successful, false otherwise.</returns>
public sealed record BalanceTransactionCommand(User Sender, User Receiver, decimal Amount) : IRequest<bool>;

internal sealed class BalanceTransactionValidator : AbstractValidator<BalanceTransactionCommand>
{
    public BalanceTransactionValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Sender)
            .Must((cmd, wallet) => wallet.Wallet.Balance >= cmd.Amount)
            .WithMessage("Sender does not have enough balance to send this amount.");

        RuleFor(x => x.Amount)
            .GreaterThan(0);
    }
}

internal sealed class BalanceTransactionHandler(IAppDbContext dbContext)
    : IRequestHandler<BalanceTransactionCommand, bool>
{
    public async Task<bool> Handle(BalanceTransactionCommand command, CancellationToken ct)
    {
        if (!command.Sender.Wallet.HasBalance(command.Amount))
            return false;

        if (!command.Sender.Wallet.TryTransfer(command.Receiver.Wallet, command.Amount))
            return false;

        dbContext.Set<User>().TryUpdateIfNotNull(command.Sender);
        dbContext.Set<User>().TryUpdateIfNotNull(command.Receiver);
        await dbContext.SaveChangesAsync(ct);

        return true;
    }
}