using Domain.ValueObjects;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Application.JsonConverters;

public sealed class MoneyJsonConverter : JsonConverter<Money>
{
    public override Money Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        Money.Parse(reader.GetString()!);

    public override void Write(Utf8JsonWriter writer, Money value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString());
}