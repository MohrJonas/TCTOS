using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using TCTOS.Abstractions.Data;

namespace TCTOS.Testing.Daemon.E2E;

public sealed class GeneralTests : TestBase
{
    [Fact]
    public void SendEmptyMessage_Errors()
    {
        using var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
        socket.Connect(new UnixDomainSocketEndPoint(SocketPath));
        socket.Send(" "u8.ToArray());
        var buffer = new byte[4096];
        var bytesRead = socket.Receive(buffer);
        var response = JsonSerializer.Deserialize<SocketResponse>(Encoding.UTF8.GetString(buffer[..bytesRead]))!;
        response.Error.Should().Be("Invalid message");
    }
}