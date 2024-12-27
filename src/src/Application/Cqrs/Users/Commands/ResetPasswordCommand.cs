using Application.Services;
using Destructurama.Attributed;
using FluentValidation;
using MediatR;

namespace Application.Cqrs.Users.Commands;

public sealed record ResetPasswordCommand : IRequest
{
    [LogMasked]
    public string NewPassword { get; set; } = null!;

    [LogMasked]
    public string ConfirmNewPassword { get; set; } = null!;

    public string Token { get; set; } = null!;
}

public sealed class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordValidator(IJwtGenerator jwtGenerator)
    {
        RuleFor(x => x.NewPassword)
            .NotEmpty().MinimumLength(12);

        RuleFor(x => x.ConfirmNewPassword)
            .NotEmpty().MinimumLength(12)
            .Equal(command => command.NewPassword).WithMessage("Passwords dont match!");

        RuleFor(x => x.Token)
            .NotEmpty()
            .Must(token => jwtGenerator.TryValidateToken(token, out _))
            .WithMessage("Invalid token!");
    }
}

internal sealed class ResetPasswordHandler(IAppDbContext dbContext, IUserVerificationTokenGenerator tokenGenerator) : IRequestHandler<ResetPasswordCommand>
{
    public async Task Handle(ResetPasswordCommand request, CancellationToken ct)
    {
        var result = tokenGenerator.ValidateToken(IUserVerificationTokenGenerator.ResetPasswordPurpose, request.Token);
        if (result is null)
            return;

        var (_, securityStamp, userId) = result.Value;

        var user = await dbContext.Users.FindAsync([userId], ct);
        if (user is null)
            throw new InvalidOperationException($"User with id {userId} not found.");

        if (user.SecurityStamp != securityStamp)
            throw new InvalidOperationException("User auth credentials changed");

        user.SetPassword(request.NewPassword);
        await dbContext.SaveChangesAsync(ct);
    }
}