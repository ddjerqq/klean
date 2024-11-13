using System.ComponentModel.DataAnnotations;

namespace Domain.Common;

public static class EnumExt
{
    public static IEnumerable<TEnum> GetIndividualFlags<TEnum>(this TEnum @enum, params TEnum[] except) where TEnum : Enum
    {
        return Enum.GetValues(typeof(TEnum))
            .Cast<TEnum>()
            .Where(value => @enum.HasFlag(value))
            .Where(value => !except.Contains(value));
    }

    public static TEnum GetAll<TEnum>() where TEnum : Enum
    {
        return Enum.GetValues(typeof(TEnum))
            .Cast<TEnum>()
            .Aggregate(
                (TEnum)Enum.ToObject(typeof(TEnum), 0),
                (current, value) => (TEnum)(object)((int)(object)current | (int)(object)value));
    }

    public static string GetDisplayName<TEnum>(this TEnum value) where TEnum : Enum
    {
        var isFlags = typeof(TEnum).IsDefined(typeof(FlagsAttribute), false);

        if (isFlags)
            return string.Join(", ", value.GetIndividualFlags().Select(flag => flag.GetAttribute<DisplayAttribute>()?.Name ?? flag.ToString()));

        return value.GetAttribute<DisplayAttribute>()?.Name ?? value.ToString();
    }

    private static TAttribute? GetAttribute<TAttribute>(this Enum value) where TAttribute : Attribute
    {
        var type = value.GetType();
        var memberInfo = type.GetMember(value.ToString());
        var attributes = memberInfo.FirstOrDefault()?.GetCustomAttributes(typeof(TAttribute), false);
        return attributes?.Length > 0 ? (TAttribute)attributes[0] : null;
    }
}