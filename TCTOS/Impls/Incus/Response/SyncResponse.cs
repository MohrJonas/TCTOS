namespace TCTOS.Impls.Incus.Response;

public record SyncResponse
{
    public required string Status { init; get; }
    public required int StatusCode { init; get; }
}

public sealed record SyncResponse<TData> : SyncResponse
{
    public required TData Metadata { init; get; }
}