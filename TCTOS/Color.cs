using System.Text.Json.Serialization;

namespace TCTOS;

public sealed record Color
{
    [JsonPropertyName("red")] public required byte Red { get; set; }

    [JsonPropertyName("green")] public required byte Green { get; set; }

    [JsonPropertyName("blue")] public required byte Blue { get; set; }
}