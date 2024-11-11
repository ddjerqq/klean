using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.JsonConverters;

/// <inheritdoc />
public sealed class UlidToStringJsonConverter : JsonConverter<Ulid>
{
    /// <inheritdoc />
    public override Ulid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Ulid.Parse(reader.GetString());

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Ulid value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString());
}