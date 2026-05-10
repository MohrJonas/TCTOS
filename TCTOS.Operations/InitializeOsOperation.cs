using TCTOS.Abstractions;
using TCTOS.Abstractions.Data;
using TCTOS.Common;

namespace TCTOS.Operations;

public static class InitializeOsOperation
{
    public static Task<Result> InitializeOsAsync(
        string poolName,
        string bridgeName,
        string nonPersistentStoragePath,
        IFileSystem fileSystem
    ) => RunCatchingAsync(async () =>
    {
        var configuration = new TctOsConfiguration
        {
            PoolName = poolName,
            BridgeName = bridgeName,
            NonPersistentRoot = nonPersistentStoragePath
        };
        (await fileSystem.SetConfigurationAsync(configuration)).ThrowIfFailed();
    });
}