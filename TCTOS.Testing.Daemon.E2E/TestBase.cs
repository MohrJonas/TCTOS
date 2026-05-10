using TCTOS.Abstractions;
using TCTOS.Impls.Local;
using Xunit.Sdk;

namespace TCTOS.Testing.Daemon.E2E;

public abstract class TestBase : IDisposable
{
    protected const string SocketPath = "/tmp/tctos.socket";
    protected readonly IIncusClient IncusClient;

    protected TestBase()
    {
        if (Environment.GetEnvironmentVariable("TCTOS_E2E") == null)
            throw new XunitException(
                "Aborted tests because the environment variable \"TCTOS_E2E\" is not set. This is a safeguard against accidental test runs. Running the E2E tests WILL REMOVE ALL CONTAINERS from your system.");
        IncusClient = new LocalUnixSocketIncusClient();
        TestHelper.RemoveAllContainers(IncusClient).Wait();
    }

    public void Dispose() => TestHelper.RemoveAllContainers(IncusClient).Wait();
}