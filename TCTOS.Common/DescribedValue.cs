namespace TCTOS.Common;

public sealed record DescribedValue<TData>(TData Data, string Explanation);