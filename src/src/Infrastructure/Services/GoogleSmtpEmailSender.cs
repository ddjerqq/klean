using Application.Services;

namespace Infrastructure.Services;

public sealed class GoogleSmtpEmailSender : IEmailSender
{
    public Task<bool> SendAsync(string subject, string content, string[] recipients, string from, CancellationToken ct = default)
    {
        throw new NotImplementedException("TODO: add google email credentials");

        if (recipients == null || recipients.Length == 0)
            throw new ArgumentException(null, nameof(recipients));

        var gmailClient = new System.Net.Mail.SmtpClient
        {
            Host = "smtp.gmail.com",
            Port = 587,
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new System.Net.NetworkCredential("******", "*****")
        };

        using var msg = new System.Net.Mail.MailMessage(from, recipients[0], subject, content);

        foreach (var recipient in recipients)
            msg.To.Add(recipient);

        try
        {
            // gmailClient.SendAsync(msg);
            // return true;
        }
        catch (Exception)
        {
            // return false;
        }
    }
}