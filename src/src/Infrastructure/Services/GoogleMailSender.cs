using System.Net;
using System.Net.Mail;
using Application.Services;
using Domain.Aggregates;
using Domain.Common;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public sealed class GoogleMailSender : IEmailSender
{
    private readonly string _username;
    private readonly SmtpClient _client;
    private readonly ILogger<GoogleMailSender> _logger;

    public GoogleMailSender(ILogger<GoogleMailSender> logger)
    {
        _logger = logger;
        _username = "GOOGLE__USERNAME".FromEnvRequired();
        var password = "GOOGLE__APP_PASSWORD".FromEnvRequired();

        _client = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(_username, password),
            Timeout = 20_000,
        };
    }

    public async Task SendAsync(string recipient, string subject, string body, CancellationToken ct = default)
    {
        var fromAddress = new MailAddress(_username);
        var toAddress = new MailAddress(recipient);

        using var msg = new MailMessage(fromAddress, toAddress);
        msg.IsBodyHtml = false;
        msg.Subject = subject;
        msg.Body = body;
        msg.IsBodyHtml = true;

        _logger.LogInformation("sending message to {recipient} {message}", recipient, msg.Body);

        try
        {
            await Task.Run(() => _client.Send(msg), ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email");
            throw;
        }
    }

    public Task SendEmailConfirmationAsync(User recipient, string callback, CancellationToken ct = default) =>
        SendAsync(recipient.Email, "Confirm your email", $"Please confirm your account by clicking this link: {callback}", ct);

    public Task SendPasswordResetAsync(User user, string callback, CancellationToken ct = default) =>
        SendAsync(user.Email, "Reset your password", $"Please reset your password by clicking this link: {callback}", ct);
}