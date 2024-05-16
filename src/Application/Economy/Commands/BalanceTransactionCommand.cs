using System.ComponentModel;
using Application.Services.Interfaces;
using Domain.Aggregates;
using FluentValidation;
using MediatR;

namespace Application.Economy.Commands;

public sealed record BalanceTransactionCommand(User Sender, User Receiver, decimal Amount) : IRequest<bool>;

[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class BalanceTransactionValidator : AbstractValidator<BalanceTransactionCommand>
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

[EditorBrowsable(EditorBrowsableState.Never)]
internal sealed class BalanceTransactionHandler(IAppDbContext dbContext)
    : IRequestHandler<BalanceTransactionCommand, bool>
{
    public async Task<bool> Handle(BalanceTransactionCommand command, CancellationToken ct)
    {
        if (!command.Sender.Wallet.HasBalance(command.Amount))
            return false;

        if (!command.Sender.Wallet.TryTransfer(command.Receiver.Wallet, command.Amount))
            return false;

        dbContext.Set<User>().Update(command.Sender);
        dbContext.Set<User>().Update(command.Receiver);
        await dbContext.SaveChangesAsync(ct);

        return true;
    }
}