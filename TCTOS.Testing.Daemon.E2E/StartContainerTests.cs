using FluentAssertions;
using TCTOS.Abstractions.Data.Messages;
using TCTOS.Client.Common;

namespace TCTOS.Testing.Daemon.E2E;

public class StartContainerTests : TestBase
{
    [Fact]
    public async Task StartContainer_ContainerDoesNotExists_Errors()
    {
        var socket = new UnixSocketWriter(SocketPath);
        var response = await socket.WriteAsync(new StartContainerSocketMessage
        {
            ContainerName = TestHelper.DefaultContainerName,
            Uid = 1000,
            Gid = 100
        });
        response.Error.Should().Contain("does not exist");
    }

    [Fact]
    public async Task StartContainer_ContainerAlreadyStarted_Ignores()
    {
        await TestHelper.CreateContainer(IncusClient);
        var startResponse = await IncusClient.StartContainerAsync(TestHelper.DefaultContainerName);
        startResponse.ThrowOnError();
        (await IncusClient.WaitForOperationAsync(startResponse.Operation!)).ThrowOnError();

        var socket = new UnixSocketWriter(SocketPath);
        var response = await socket.WriteAsync(new StartContainerSocketMessage
        {
            ContainerName = TestHelper.DefaultContainerName,
            Uid = 1000,
            Gid = 100
        });
        response.Error.Should().BeNull();
    }

    [Fact]
    public async Task StartContainer_ContainerStopped_StartsContainer()
    {
        await TestHelper.CreateContainer(IncusClient);

        var socket = new UnixSocketWriter(SocketPath);
        var response = await socket.WriteAsync(new StartContainerSocketMessage
        {
            ContainerName = TestHelper.DefaultContainerName,
            Uid = 1000,
            Gid = 100
        });
        response.Error.Should().BeNull();

        (await IncusClient.GetContainerAsync(TestHelper.DefaultContainerName)).Metadata.Status.Should().Be("Running");
    }
}