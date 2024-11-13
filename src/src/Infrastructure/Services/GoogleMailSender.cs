using System.Net;
using System.Net.Mail;
using Application.Services;
using Domain.Common;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public sealed class GoogleMailSender : IEmailSender
{
    private readonly SmtpClient _client;
    private readonly ILogger<GoogleMailSender> _logger;

    public GoogleMailSender(ILogger<GoogleMailSender> logger)
    {
        _logger = logger;

        var username = "SMTP__USERNAME".FromEnvRequired();
        var password = "SMTP__PASSWORD".FromEnvRequired();
        var port = int.Parse("SMTP__PORT".FromEnvRequired());
        var host = "SMTP__HOST".FromEnvRequired();

        _client = new SmtpClient
        {
            Host = host,
            Port = port,
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(username, password),
            DeliveryMethod = SmtpDeliveryMethod.Network,
            Timeout = 20_000,
        };
    }

    public async Task SendAsync(string from, string recipient, string subject, string body, CancellationToken ct = default)
    {
        var fromAddress = new MailAddress(from);
        var toAddress = new MailAddress(recipient);

        using var msg = new MailMessage(fromAddress, toAddress);
        msg.IsBodyHtml = false;
        msg.Subject = subject;
        msg.Body = body;
        msg.IsBodyHtml = true;

        try
        {
            await _client.SendMailAsync(msg, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email");
            throw;
        }
    }
}