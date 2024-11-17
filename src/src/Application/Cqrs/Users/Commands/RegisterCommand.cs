using Application.Common;
using Application.Services;
using Domain.Aggregates;
using Domain.ValueObjects;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Cqrs.Users.Commands;

public sealed record RegisterCommand(string Email, string FullName, string Password) : IRequest<(User User, string Token)>;

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
            PasswordHash = BC.EnhancedHashPassword(request.Password),
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(ct);

        var token = jwtGenerator.GenerateToken(user.GetClaims());
        return (user, token);
    }
}