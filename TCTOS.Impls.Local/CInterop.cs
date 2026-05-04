using System.Runtime.InteropServices;

namespace TCTOS.Impls.Local;

public static partial class CInterop
{
    [LibraryImport("libc")]
    public static partial int getuid();

    [LibraryImport("libc")]
    public static partial int getgid();
}