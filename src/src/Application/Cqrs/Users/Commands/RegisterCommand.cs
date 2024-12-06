using Application.Common;
using Application.Services;
using Destructurama.Attributed;
using Domain.Aggregates;
using Domain.ValueObjects;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Cqrs.Users.Commands;

public sealed record RegisterCommand : IRequest<(User User, string Token)>
{
    [LogMasked]
    public string Email { get; set; } = default!;

    [LogMasked]
    public string FullName { get; set; } = default!;

    [LogMasked]
    public string Password { get; set; } = default!;
}

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator(IAppDbContext dbContext)
    {
        RuleFor(c => c.Email).EmailAddress().NotEmpty();

        RuleFor(c => c.FullName)
            .NotEmpty()
            .MinimumLength(3).MaximumLength(32);

        RuleFor(c => c.Password)
            .NotEmpty()
            .MinimumLength(8).MaximumLength(256);

        RuleSet("async", () =>
        {
            RuleFor(c => c.Email)
                .MustAsync(async (s, ct) => await dbContext.Users.CountAsync(u => u.Email == s.ToLowerInvariant(), ct) == 0)
                .WithMessage("Email already in use");

            RuleFor(c => c.FullName)
                .MustAsync(async (s, ct) => await dbContext.Users.CountAsync(u => u.FullName == s.ToLowerInvariant(), ct) == 0)
                .WithMessage("Username already in use");
        });
    }
}

internal sealed class RegisterCommandHandler(IAppDbContext dbContext, IJwtGenerator jwtGenerator) : IRequestHandler<RegisterCommand, (User User, string Token)>
{
    public async Task<(User User, string Token)> Handle(RegisterCommand request, CancellationToken ct)
    {
        var user = new User(UserId.New())
        {
            Email = request.Email.ToLowerInvariant(),
            FullName = request.FullName,
            Role = Role.User,
            AvatarUrl = ClaimsPrincipalExt.GetDefaultAvatar(request.FullName),
            PasswordHash = BC.EnhancedHashPassword(request.Password),
            SecurityStamp = Guid.NewGuid().ToString(),
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(ct);

        var token = jwtGenerator.GenerateToken(user.GetClaims());
        return (user, token);
    }
}