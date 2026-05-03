using System.Text.Json;
using System.Text.Json.Serialization;

namespace TCTOS.Impls.Incus.Conversion;

public sealed class PipeSeparatedStringToStringArrayConverter : JsonConverter<string[]>
{
    public override string[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var stringValue = reader.GetString();
        return stringValue?.Split("|", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
    }

    public override void Write(Utf8JsonWriter writer, string[] value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(string.Join(" | ", value));
    }
}