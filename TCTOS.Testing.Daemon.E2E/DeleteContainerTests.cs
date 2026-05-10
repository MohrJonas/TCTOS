using FluentAssertions;
using TCTOS.Abstractions.Data.Messages;
using TCTOS.Client.Common;

namespace TCTOS.Testing.Daemon.E2E;

public class DeleteContainerTests : TestBase
{
    [Fact]
    public async Task DeleteContainer_DeletesContainer()
    {
        await TestHelper.CreateContainer(IncusClient);
        var socket = new UnixSocketWriter(SocketPath);
        var response = await socket.WriteAsync(new DeleteContainerSocketMessage(TestHelper.DefaultContainerName));
        response.Error.Should().BeNull();
    }

    [Fact]
    public async Task DeleteContainer_ContainerDoesNotExist_Errors()
    {
        var socket = new UnixSocketWriter(SocketPath);
        var response = await socket.WriteAsync(new DeleteContainerSocketMessage(TestHelper.DefaultContainerName));
        response.Error.Should().Contain("does not exist");
    }
}