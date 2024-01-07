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
    /// <param name="text">text</param>
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
    /// <param name="key">The key for the environmental variable</param>
    /// <param name="default">Optional default, to return if the value is not found, otherwise an exception is thrown</param>
    /// <returns>The value, or the default (if supplied) otherwise throws an exception</returns>
    /// <exception cref="Exception">If no default is provided, and the environmental variable is not found</exception>
    public static string GetFromEnvironment(this string key, string? @default = null)
    {
        var value = Environment.GetEnvironmentVariable(key);
        return value
               ?? @default
               ?? throw new Exception($"{key} is not present in the environment");
    }
}