using Application.Common.Interfaces;
using Domain.Aggregates;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Auth;

/// <summary>
/// User login command
/// </summary>
public sealed record UserLoginCommand(string Username, string Password, bool RememberMe)
    : IRequest<User?>;

internal sealed class UserLoginValidator : AbstractValidator<UserLoginCommand>
{
    public UserLoginValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Username)
            .Length(3, 16)
            .Matches(@"^[a-zA-Z0-9\._]{3,32}$")
            .WithMessage("Username must contain only alphanumeric characters, underscores and dots.");

        RuleFor(x => x.Password)
            .Length(8, 64)
            .NotEmpty();
    }
}

internal sealed class UserLoginHandler(IAppDbContext dbContext)
    : IRequestHandler<UserLoginCommand, User?>
{
    public async Task<User?> Handle(UserLoginCommand command, CancellationToken ct)
    {
        var user = await dbContext.Set<User>()
            .FirstOrDefaultAsync(x => x.Username == command.Username, ct);

        if (user is null)
            return null;

        if (!BC.Verify(command.Password, user.PasswordHash))
            return null;

        return user;
    }
}