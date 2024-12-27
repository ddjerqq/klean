using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Cysharp.Serialization.Json;
using Generated;

namespace Application.JsonConverters;

public static class ApplicationJsonConstants
{
    public static readonly Lazy<JsonSerializerOptions> Options = new(() =>
    {
        #if DEBUG
        const bool indent = true;
        #else
        const bool indent = false;
        #endif

        var opt = new JsonSerializerOptions
        {
            WriteIndented = indent,
            IndentSize = 2,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            Converters =
            {
                new JsonStringEnumConverter(),
                new UlidJsonConverter(),
            },
        };

        var types =
            from Type type in Assembly.GetAssembly(typeof(ApplicationJsonConstants))!.GetTypes()
            let baseType = type.BaseType
            where baseType?.IsGenericType ?? false
            where baseType?.GetGenericTypeDefinition() == typeof(JsonConverter<>)
            let idType = baseType.GetGenericArguments().First()
            select type;

        foreach (var type in types)
            opt.Converters.Add(Activator.CreateInstance(type) as JsonConverter ?? throw new InvalidOperationException());

        opt.Converters.ConfigureGeneratedConverters();

        return opt;
    });
}