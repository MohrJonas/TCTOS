using System.Text.Json.Serialization;

namespace TCTOS.Abstractions.Incus.Response;

public sealed record ErrorResponse
{
    [JsonPropertyName("error")] public required string Error { init; get; }

    [JsonPropertyName("error_code")] public required int ErrorCode { init; get; }
}