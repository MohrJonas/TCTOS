using TCTOS.Abstractions;
using TCTOS.Util;

namespace TCTOS.Impls;

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