using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using TCTOS.Abstractions.Data;

namespace TCTOS.Client.Common;

public sealed class UnixSocketWriter(string socketPath)
{
    private async Task<string> WriteInternalAsync(SocketMessage message)
    {
        using var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
        await socket.ConnectAsync(new UnixDomainSocketEndPoint(socketPath));
        socket.Send(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message)));
        var buffer = new byte[4096];
        var bytesRead = await socket.ReceiveAsync(buffer);
        return Encoding.UTF8.GetString(buffer[..bytesRead]);
    }
    
    public async Task<SocketResponse> WriteAsync(SocketMessage message) 
        => JsonSerializer.Deserialize<SocketResponse>(await WriteInternalAsync(message))!;

    public async Task<SocketResponse<TData>> WriteAsync<TData>(SocketMessage message) where TData : class
        => JsonSerializer.Deserialize<SocketResponse>(await WriteInternalAsync(message))!.Into<TData>();
}