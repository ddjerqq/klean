namespace Application.Services;

public interface IEmailSender
{
    /// <summary>
    /// Sends an email with the specified subject, content, recipients, and from address.
    /// </summary>
    public Task<bool> SendAsync(string subject, string content, string[] recipients, string from, CancellationToken ct = default);
}