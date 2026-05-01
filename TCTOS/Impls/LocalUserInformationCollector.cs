using TCTOS.Abstractions;
using TCTOS.Util;

namespace TCTOS.Impls;

public sealed class LocalUserInformationCollector : IUserInformationCollector
{
    public uint GetUid() => (uint)CInterop.getuid();

    public uint GetGid() => (uint)CInterop.getgid();
}