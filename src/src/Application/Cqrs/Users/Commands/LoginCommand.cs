using Application.Common;
using Application.Services;
using Domain.Aggregates;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Cqrs.Users.Commands;

public abstract record LoginResponse
{
    public sealed record Success(string Token, User User) : LoginResponse;
    // public sealed record TwoFactorRequired(string Token, User User) : LoginResponse;
    public sealed record Failure : LoginResponse;
}

public sealed record LoginCommand(string Email, string Password) : IRequest<LoginResponse>;

internal sealed class LoginCommandHandler(IAppDbContext dbContext, IJwtGenerator jwtGenerator) : IRequestHandler<LoginCommand, LoginResponse>
{
    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await dbContext.Users.SingleOrDefaultAsync(u => u.Email == request.Email.ToUpperInvariant(), ct);
        if (user is null || !BC.EnhancedVerify(request.Password, user.PasswordHash))
            return new LoginResponse.Failure();

        var token = jwtGenerator.GenerateToken(user.GetClaims());
        return new LoginResponse.Success(token, user);
    }
}