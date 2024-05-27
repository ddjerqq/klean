using System.ComponentModel;

namespace Domain.Common;

// TODO needs improvement, the generic should not
public sealed class StrongIdHelper<TId, TValue> where TId : struct
{
    public static string Serialize(TValue value) =>
        $"{GetPrefix(typeof(TId).Name)}_{value?.ToString()?.ToLower() ?? string.Empty}";

    public static TId? Deserialize(string? id, Func<string, string>? valuePreprocess)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

        var prefix = $"{GetPrefix(typeof(TId).Name)}_";
        if (!id.StartsWith(prefix))
            return null;

        var rawValue = valuePreprocess?.Invoke(id[prefix.Length..]) ?? id[prefix.Length..];

        try
        {
            if (TypeDescriptor.GetConverter(typeof(TValue)).ConvertFromString(rawValue) is TValue value)
                return (TId?)Activator.CreateInstance(typeof(TId), value);
        }
        // ReSharper disable once EmptyGeneralCatchClause
        catch
        {
        }

        return null;
    }

    private static string GetPrefix(string idTypeName)
    {
        var id = idTypeName.Replace("id", string.Empty, StringComparison.InvariantCultureIgnoreCase);
        return id.ToSnakeCase();
    }
}