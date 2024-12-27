using System.Text.Json.Serialization;

namespace Application.Services;

public sealed record RecaptchaResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("challenge_ts")]
    public DateTime ChallengeTimestamp { get; set; }

    [JsonPropertyName("hostname")]
    public string Hostname { get; set; } = null!;

    [JsonPropertyName("error-codes")]
    public IEnumerable<string> ErrorCodes { get; set; } = [];
}

public interface IRecaptchaVerifier
{
    public Task<RecaptchaResponse> VerifyAsync(string recaptchaResponse, CancellationToken ct = default);
}