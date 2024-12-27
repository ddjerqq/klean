using Application.Services;
using Destructurama.Attributed;
using Domain.Aggregates;
using EntityFrameworkCore.DataProtection.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Cqrs.Users.Commands;

public sealed record ResendEmailConfirmationCommand : IRequest
{
    [LogMasked]
    public string Email { get; set; } = "";
}

public sealed class ResendEmailConfirmationValidator : AbstractValidator<ResendEmailConfirmationCommand>
{
    public ResendEmailConfirmationValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}

internal sealed class ResendEmailConfirmationHandler(IAppDbContext dbContext, ILogger<ResendEmailConfirmationHandler> logger, IEmailSender emailSender, IUserVerificationTokenGenerator tokenGenerator) : IRequestHandler<ResendEmailConfirmationCommand>
{
    public async Task Handle(ResendEmailConfirmationCommand request, CancellationToken ct)
    {
        var user = await dbContext.Users.WherePdEquals(nameof(User.Email), request.Email.ToLowerInvariant()).FirstOrDefaultAsync(ct);

        // use not found or already confirmed
        if (user is null || user.EmailConfirmed)
            return;

        var callbackUrl = tokenGenerator.GenerateConfirmEmailCallbackUrl(user);
        logger.LogInformation("User {UserId} registered, sending confirmation link: {ConfirmationLink}", user.Id, callbackUrl);
        await emailSender.SendEmailConfirmationAsync(user, callbackUrl, ct);
    }
}