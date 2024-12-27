using Application.Services;
using Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Cqrs.Users.Events;

internal sealed class UserRegisteredEventHandler(
    ILogger<UserRegisteredEventHandler> logger,
    IUserVerificationTokenGenerator tokenGenerator,
    IAppDbContext dbContext,
    IEmailSender emailSender)
    : INotificationHandler<UserRegistered>
{
    public async Task Handle(UserRegistered notification, CancellationToken ct)
    {
        var user = await dbContext.Users.FindAsync([notification.UserId], ct)
                   ?? throw new InvalidOperationException($"Failed to load the user from the database, user with id: {notification.UserId} not found");

        var callbackUrl = tokenGenerator.GenerateConfirmEmailCallbackUrl(user);
        logger.LogInformation("User {UserId} registered, sending confirmation link: {ConfirmationLink}", user.Id, callbackUrl);
        await emailSender.SendEmailConfirmationAsync(user, callbackUrl, ct);
    }
}