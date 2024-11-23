using Application.Common;
using Application.Services;
using Destructurama.Attributed;
using Domain.Aggregates;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Cqrs.Users.Commands;

public abstract record LoginResponse
{
    public sealed record Success(string Token, User User) : LoginResponse;
    public sealed record Failure : LoginResponse;
}

public sealed record LoginCommand : IRequest<LoginResponse>
{
    [LogMasked]
    public string Email { get; set; } = default!;

    [LogMasked]
    public string Password { get; set; } = default!;
}

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}

internal sealed class LoginCommandHandler(IAppDbContext dbContext, IJwtGenerator jwtGenerator) : IRequestHandler<LoginCommand, LoginResponse>
{
    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await dbContext.Users.SingleOrDefaultAsync(u => u.Email == request.Email.ToLowerInvariant(), ct);
        if (user is null || !BC.EnhancedVerify(request.Password, user.PasswordHash))
            return new LoginResponse.Failure();

        var token = jwtGenerator.GenerateToken(user.GetClaims());
        return new LoginResponse.Success(token, user);
    }
}