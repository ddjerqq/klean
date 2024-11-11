using System.ComponentModel;
using Application.Services;
using Domain.Aggregates;
using FluentValidation;
using Generated;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Commands;

public sealed record UserRegisterCommand(string Username, string Email, string Password) : IRequest<User>;

[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class UserRegisterCommandValidator : AbstractValidator<UserRegisterCommand>
{
    public UserRegisterCommandValidator(IAppDbContext dbContext)
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .Length(3, 32)
            .Matches(@"[a-z0-9_\.]{3,32}")
            .WithMessage("Username may contain lowercase latin letters, numbers, underscores and the period character ('.')");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Invalid email address");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(12)
            .WithMessage("the password must be at least 12 characters long. Think of a passphrase (e.g. 'purity filter vertigo away')");

        RuleSet("async", () =>
        {
            RuleFor(x => x.Username)
                .MustAsync(async (username, ct) =>
                {
                    var userCount = await dbContext.Users.CountAsync(x => x.Username == username, ct);
                    return userCount == 0;
                })
                .WithMessage("Username is taken");

            RuleFor(x => x.Email)
                .MustAsync(async (email, ct) =>
                {
                    var userCount = await dbContext.Users.CountAsync(x => x.Email == email, ct);
                    return userCount == 0;
                })
                .WithMessage("Email is taken");
        });
    }
}

[EditorBrowsable(EditorBrowsableState.Never)]
internal sealed class UserRegisterCommandHandler(IAppDbContext dbContext, IDateTimeProvider dateTimeProvider) : IRequestHandler<UserRegisterCommand, User>
{
    public async Task<User> Handle(UserRegisterCommand request, CancellationToken ct)
    {
        var user = new User(UserId.New())
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BC.EnhancedHashPassword(request.Password),
            Created = dateTimeProvider.UtcNow,
            CreatedBy = "system",
            LastModified = null,
            LastModifiedBy = null,
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(ct);

        return user;
    }
}