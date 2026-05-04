using System.Text.Json;
using System.Text.Json.Serialization;

namespace TCTOS.Abstractions.Incus.Conversion;

public sealed class ZeroToNullConverter : JsonConverter<int?>
{
    public override int? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var intValue = reader.GetInt32();
        if (intValue == 0)
            return null;
        return intValue;
    }

    public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(writer.ToString());
    }
}