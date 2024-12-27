using Domain.Aggregates;

namespace Application.Services;

public interface IEmailSender
{
    /// <summary>
    ///     Sends an email with the specified subject, content, recipients, and from address.
    /// </summary>
    protected Task SendAsync(string recipient, string subject, string content, CancellationToken ct = default);

    /// <summary>
    /// Sends an Email confirmation link to the specified user
    /// </summary>
    public Task SendEmailConfirmationAsync(User recipient, string callback, CancellationToken ct = default);

    /// <summary>
    /// Sends a password reset link to the specified user
    /// </summary>
    public Task SendPasswordResetAsync(User user, string callback, CancellationToken ct = default);
}