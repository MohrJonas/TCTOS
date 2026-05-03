using System.Text.Json.Serialization;
using TCTOS.Impls.Incus.Conversion;

namespace TCTOS.Impls.Incus.Response;

public record ResponseBase
{
    [JsonPropertyName("type")]
    [JsonConverter(typeof(StringToEnumConverter<ResponseType>))]
    public ResponseType Type { init; get; }

    [JsonPropertyName("status")]
    [JsonConverter(typeof(EmptyStringToNullConverter))]
    public string? Status { init; get; }

    [JsonPropertyName("status_code")]
    [JsonConverter(typeof(ZeroToNullConverter))]
    public int? StatusCode { init; get; }

    [JsonPropertyName("operation")]
    [JsonConverter(typeof(EmptyStringToNullConverter))]
    public string? Operation { init; get; }

    [JsonPropertyName("error_code")]
    [JsonConverter(typeof(ZeroToNullConverter))]
    public int? ErrorCode { init; get; }

    [JsonPropertyName("error")]
    [JsonConverter(typeof(EmptyStringToNullConverter))]
    public string? Error { init; get; }

    public void ThrowOnError()
    {
        if (Error != null)
            throw new IncusClientException($"{Error} (Error Code {ErrorCode})");
    }

    public bool IsError()
    {
        return Error != null;
    }
}

public record ResponseBase<TData> : ResponseBase
{
    [JsonPropertyName("metadata")] public required TData Metadata { init; get; }
}