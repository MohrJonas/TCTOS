using System.CommandLine;
using TCTOS.Abstractions;
using TCTOS.Operations;

namespace TCTOS.Console.Commands;

public static class InitializeOsCommandArguments
{
    public static readonly Argument<string> PoolNameArgument = new("pool")
    {
        Description = "Name of the storage pool to use"
    };

    public static readonly Argument<string> BridgeNameArgument = new("bridge")
    {
        Description = "Name of the network bridge to use"
    };

    public static readonly Argument<string> NonPersistentStoragePathArgument = new("non_persistent_storage")
    {
        Description = "Path to the non-persistent storage directory, e.g. a tmpfs mount"
    };
}

public sealed class InitializeOsCommand(DiContainer container)
    : CommandBase("init", "Create the os configuration", container, arguments:
    [
        InitializeOsCommandArguments.PoolNameArgument,
        InitializeOsCommandArguments.BridgeNameArgument,
        InitializeOsCommandArguments.NonPersistentStoragePathArgument
    ])
{
    protected override async Task RunAsync(ParseResult parseResult, DiContainer container, CancellationToken token)
    {
        var fileSystem = container.Get<IFileSystem>();
        (await InitializeOsOperation.InitializeOsAsync(
            parseResult.GetRequiredValue(InitializeOsCommandArguments.PoolNameArgument),
            parseResult.GetRequiredValue(InitializeOsCommandArguments.BridgeNameArgument),
            parseResult.GetRequiredValue(InitializeOsCommandArguments.NonPersistentStoragePathArgument),
            fileSystem
        )).ThrowIfFailed();
    }
}