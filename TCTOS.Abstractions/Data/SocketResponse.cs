using System.Text.Json;
using System.Text.Json.Serialization;

namespace TCTOS.Abstractions.Data;

public sealed record SocketResponse<TData>(string? Error, TData? Data);

public sealed class SocketResponse(string? error = null, object? data = null)
{
    public SocketResponse() : this(null)
    {
    }

    [JsonPropertyName("error")] public string? Error { init; get; } = error;
    [JsonPropertyName("Data")] public JsonElement? Data { init; get; } = JsonSerializer.SerializeToElement(data);

    public SocketResponse<TData> Into<TData>() where TData : class
    {
        return new SocketResponse<TData>(
            Error,
            Data?.Deserialize<TData>()
        );
    }
}