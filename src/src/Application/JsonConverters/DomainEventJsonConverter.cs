using System.Text.Json;
using System.Text.Json.Serialization;
using Domain.Abstractions;

namespace Application.JsonConverters;

public sealed class DomainEventJsonConverter : JsonConverter<IDomainEvent>
{
    public override IDomainEvent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var jsonDocument = JsonDocument.ParseValue(ref reader);

        var root = jsonDocument.RootElement;
        var type = root.GetProperty("Type").GetString();
        var data = root.GetProperty("Data").GetRawText();

        var eventType = Type.GetType(type!);
        if (eventType is null)
            throw new InvalidOperationException($"Type '{type}' not found.");

        return (IDomainEvent)JsonSerializer.Deserialize(data, eventType, options)!;
    }

    public override void Write(Utf8JsonWriter writer, IDomainEvent value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("Type", value.GetType().AssemblyQualifiedName);
        writer.WritePropertyName("Data");
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
        writer.WriteEndObject();
    }
}