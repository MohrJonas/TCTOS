using System.Runtime.InteropServices;

namespace TCTOS.Util;

public static partial class CInterop
{
    [LibraryImport("libc")]
    public static partial int getuid();

    [LibraryImport("libc")]
    public static partial int getgid();
}