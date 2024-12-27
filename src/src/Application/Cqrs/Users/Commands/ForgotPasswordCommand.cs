using Application.Services;
using Destructurama.Attributed;
using Domain.Aggregates;
using EntityFrameworkCore.DataProtection.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Cqrs.Users.Commands;

public sealed record ForgotPasswordCommand : IRequest
{
    [LogMasked]
    public string Email { get; set; } = "";
}

public sealed class ForgotPasswordValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}

internal sealed class ForgotPasswordCommandHandler(IAppDbContext dbContext, IEmailSender emailSender, IUserVerificationTokenGenerator tokenGenerator) : IRequestHandler<ForgotPasswordCommand>
{
    public async Task Handle(ForgotPasswordCommand request, CancellationToken ct)
    {
        var user = await dbContext.Users.WherePdEquals(nameof(User.Email), request.Email.ToLowerInvariant()).FirstOrDefaultAsync(ct);

        if (user is null || !user.EmailConfirmed)
            return;

        var callbackUrl = tokenGenerator.GeneratePasswordResetCallbackUrl(user);
        await emailSender.SendPasswordResetAsync(user, callbackUrl, ct);
    }
}