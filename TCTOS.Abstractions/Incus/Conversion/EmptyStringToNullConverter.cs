using System.Text.Json;
using System.Text.Json.Serialization;

namespace TCTOS.Abstractions.Incus.Conversion;

public sealed class EmptyStringToNullConverter : JsonConverter<string?>
{
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var stringValue = reader.GetString();
        return string.IsNullOrEmpty(stringValue) ? null : stringValue;
    }

    public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}