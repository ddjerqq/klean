using Application.Services;
using Domain.Aggregates;
using FluentValidation;
using MediatR;

namespace Application.Cqrs.Users.Commands;

public sealed record ConfirmEmailCommand(string Token) : IRequest<User?>;

public sealed class ConfirmEmailValidator : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailValidator()
    {
        RuleFor(x => x.Token).NotEmpty();
    }
}

internal sealed class ConfirmEmailHandler(IAppDbContext dbContext, IUserVerificationTokenGenerator tokenGenerator) : IRequestHandler<ConfirmEmailCommand, User?>
{
    public async Task<User?> Handle(ConfirmEmailCommand request, CancellationToken ct)
    {
        var result = tokenGenerator.ValidateToken(IUserVerificationTokenGenerator.ConfirmEmailPurpose, request.Token);
        if (result is null)
            return null;

        var (_, securityStamp, userId) = result.Value;

        var user = await dbContext.Users.FindAsync([userId], ct);
        if (user is null)
            throw new InvalidOperationException($"User with id {userId} not found.");

        if (user.SecurityStamp != securityStamp)
            throw new InvalidOperationException("User auth credentials changed");

        user.EmailConfirmed = true;
        await dbContext.SaveChangesAsync(ct);
        return user;
    }
}