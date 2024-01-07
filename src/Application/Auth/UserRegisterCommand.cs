using Application.Common.Interfaces;
using Domain.Aggregates;
using Domain.Events;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Application.Auth;

/// <summary>
/// User register command
/// </summary>
public sealed record UserRegisterCommand(string Username, string Email, string Password)
    : IRequest<bool>
{
    private readonly Guid _id = Guid.NewGuid();

    /// <summary>
    /// The created user
    /// </summary>
    [JsonIgnore]
    public User User => new()
    {
        Id = _id,
        Username = Username,
        Email = Email,
    };
}

internal sealed class UserRegisterValidator : AbstractValidator<UserRegisterCommand>
{
    public UserRegisterValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Username)
            .NotEmpty()
            .Matches(@"^[a-zA-Z0-9._]{3,16}$")
            .WithMessage(
                "Username must be between 3 and 16 characters long and contain only alphanumeric characters, underscores and dots.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .Matches(@"^[\w-.]+@([\w-]+\.)+[\w-]{2,4}$")
            .WithMessage("Email must be a valid email address.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$")
            .WithMessage(
                "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one number and one special character.");
    }
}

internal sealed class UserRegisterHandler(IAppDbContext dbContext)
    : IRequestHandler<UserRegisterCommand, bool>
{
    public async Task<bool> Handle(UserRegisterCommand command, CancellationToken ct)
    {
        var user = command.User;

        var exists = await dbContext.Set<User>()
            .AnyAsync(x => x.Username == command.Username || x.Email == command.Email, ct);

        if (exists)
            return false;

        user.AddDomainEvent(new UserCreatedEvent(user.Id));

        await dbContext
            .Set<User>()
            .AddAsync(user, ct);

        await dbContext.SaveChangesAsync(ct);

        return true;
    }
}