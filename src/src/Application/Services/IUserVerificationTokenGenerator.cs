using Domain.Aggregates;
using Domain.Common;
using Microsoft.AspNetCore.WebUtilities;

namespace Application.Services;

public interface IUserVerificationTokenGenerator
{
    public const string ConfirmEmailPurpose = "confirm_email";
    public const string ResetPasswordPurpose = "reset_password";

    private static string WebAppDomain => "WEB_APP__DOMAIN".FromEnvRequired();
    protected string GenerateToken(User user, string purpose);

    /// <summary>
    /// Generates a link to `auth/confirm_email` with the token set in the query parameters
    /// </summary>
    public string GenerateConfirmEmailCallbackUrl(User user) =>
        QueryHelpers.AddQueryString($"https://{WebAppDomain}/auth/{ConfirmEmailPurpose}",
            new Dictionary<string, string?> { ["token"] = GenerateToken(user, ConfirmEmailPurpose) });

    /// <summary>
    /// Generates a link to `auth/reset_password` with the token set in the query parameters
    /// </summary>
    public string GeneratePasswordResetCallbackUrl(User user) =>
        QueryHelpers.AddQueryString($"https://{WebAppDomain}/auth/{ResetPasswordPurpose}",
            new Dictionary<string, string?> { ["token"] = GenerateToken(user, ResetPasswordPurpose) });


    /// <summary>
    /// Validates that the jwt is valid and the token's purpose matches
    /// </summary>
    /// <remarks>
    /// Please note it is your responsibility to validate the security_stamp and the sid
    /// </remarks>
    public (string Purpose, string SecurityStamp, UserId UserId)? ValidateToken(string purpose, string token);
}