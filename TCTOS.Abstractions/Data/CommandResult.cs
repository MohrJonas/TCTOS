namespace TCTOS.Abstractions.Data;

public sealed record CommandResult(string? Stdout, string? Stderr, int? ExitCode);