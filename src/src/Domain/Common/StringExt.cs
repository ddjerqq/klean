namespace Domain.Common;

public static class StringExt
{
    public static string? FromEnv(this string key)
    {
        return Environment.GetEnvironmentVariable(key);
    }

    public static string FromEnv(this string key, string value)
    {
        return Environment.GetEnvironmentVariable(key) ?? value;
    }

    public static string FromEnvRequired(this string key)
    {
        return Environment.GetEnvironmentVariable(key)
               ?? throw new InvalidOperationException($"Environment variable not found: {key}");
    }
}