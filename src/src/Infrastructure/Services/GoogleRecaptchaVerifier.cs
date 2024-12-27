using System.Net.Http.Json;
using Application.Services;
using Domain.Common;
using Microsoft.AspNetCore.WebUtilities;

namespace Infrastructure.Services;

public sealed class GoogleRecaptchaVerifier(HttpClient http) : IRecaptchaVerifier
{
    private const string Url = "https://www.google.com/recaptcha/api/siteverify";
    private static string RecaptchaSecretKey => "RECAPTCHA__SECRET_KEY".FromEnvRequired();

    public async Task<RecaptchaResponse> VerifyAsync(string recaptchaResponse, CancellationToken ct = default)
    {
        var query = new Dictionary<string, string?>
        {
            ["secret"] = RecaptchaSecretKey,
            ["response"] = recaptchaResponse,
        };
        var url = QueryHelpers.AddQueryString(Url, query);

        var response = await http.PostAsync(url, null, ct);
        var responseData = await response.Content.ReadFromJsonAsync<RecaptchaResponse>(ct);
        return responseData!;
    }
}