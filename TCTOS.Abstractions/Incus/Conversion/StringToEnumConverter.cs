using System.Text.Json;
using System.Text.Json.Serialization;

namespace TCTOS.Abstractions.Incus.Conversion;

public sealed class StringToEnumConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct
{
    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return Enum.Parse<TEnum>(reader.GetString()!, true);
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString()?.ToLower());
    }
}