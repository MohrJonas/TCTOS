using TCTOS.Abstractions;

namespace TCTOS.Impls.Local;

public sealed class LocalUserInformationCollector : IUserInformationCollector
{
    public uint GetUid()
    {
        return (uint)CInterop.getuid();
    }

    public uint GetGid()
    {
        return (uint)CInterop.getgid();
    }
}