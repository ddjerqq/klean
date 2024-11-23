namespace Domain.Common;

public static class StringExt
{
    public static (string First, string Last)? SplitName(this string name) => name.Split(' ') switch
    {
        [var first, var last] => (first, last),
        _ => null,
    };

    public static string? Initials(this string name) => name.SplitName() switch
    {
        var (first, last) => $"{first[0]}{last[0]}",
        _ => null,
    };

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