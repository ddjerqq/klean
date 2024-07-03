using System.ComponentModel;
using Application.Services;
using Domain.Aggregates;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Commands;

public sealed record UserLoginCommand(string? Username, string? Email, string Password) : IRequest<User?>;

[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class UserLoginCommandValidator : AbstractValidator<UserLoginCommand>
{
    public UserLoginCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Username)
            .Length(3, 32)
            .Matches(@"[a-z0-9_\.]{3,32}")
            .WithMessage("Username may contain lowercase latin letters, numbers, underscores and the period character ('.')");

        When(x => string.IsNullOrWhiteSpace(x.Email), () =>
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage("Either the username or the email is required");
        });

        RuleFor(x => x.Email)
            .EmailAddress();

        When(x => string.IsNullOrWhiteSpace(x.Username), () =>
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Either the username or the email is required");
        });

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(12)
            .WithMessage("the password must be at least 12 characters long. Think of a passphrase (e.g. 'purity filter vertigo away')");
    }
}

[EditorBrowsable(EditorBrowsableState.Never)]
internal sealed class UserLoginCommandHandler(IAppDbContext dbContext) : IRequestHandler<UserLoginCommand, User?>
{
    public async Task<User?> Handle(UserLoginCommand request, CancellationToken ct)
    {
        var user = await dbContext.Set<User>().SingleOrDefaultAsync(x => x.Username == request.Username || x.Email == request.Email, ct);

        if (user is null || !BC.EnhancedVerify(request.Password, user.PasswordHash))
            return null;

        return user;
    }
}