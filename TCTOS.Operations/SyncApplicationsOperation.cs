using TCTOS.Abstractions;
using TCTOS.Common;

namespace TCTOS.Operations;

public static class SyncApplicationsOperation
{
    public static Task<Result> SyncApplicationsAsync(
        string containerName,
        IIncusFileSystem incusFileSystem,
        IComputer computer
    ) => RunCatchingAsync(async () =>
    {
        
    });
}