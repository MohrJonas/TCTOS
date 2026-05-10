using TCTOS.Abstractions;
using TCTOS.Impls.Local;

namespace TCTOS.Testing.Daemon.E2E;

public abstract class TestBase : IDisposable
{
    protected const string SocketPath = "/tmp/tctos.socket";
    protected readonly IIncusClient IncusClient;

    protected TestBase()
    {
        IncusClient = new LocalUnixSocketIncusClient();
        TestHelper.RemoveAllContainers(IncusClient).Wait();
    }

    public void Dispose() => TestHelper.RemoveAllContainers(IncusClient).Wait();
}