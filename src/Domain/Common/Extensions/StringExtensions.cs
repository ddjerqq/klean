using System.Text;

namespace Domain.Common.Extensions;

/// <summary>
/// This is an example of how to implement extensions in the codebase
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Converts a string to snake case.
    /// </summary>
    public static string ToSnakeCase(this string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        if (text.Length < 2)
            return text;

        var sb = new StringBuilder();
        sb.Append(char.ToLowerInvariant(text[0]));

        for (int i = 1; i < text.Length; ++i)
        {
            char c = text[i];
            if (char.IsUpper(c))
            {
                sb.Append('_');
                sb.Append(char.ToLowerInvariant(c));
            }
            else
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Gets a value from the environment.
    /// </summary>
    public static string? FromEnv(this string key) =>
        Environment.GetEnvironmentVariable(key);

    /// <summary>
    /// Gets a value from the environment. If the value is not found, the default value is returned.
    /// </summary>
    public static string FromEnv(this string key, string value) =>
        Environment.GetEnvironmentVariable(key) ?? value;


}