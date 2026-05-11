using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using TCTOS.Abstractions;
using TCTOS.Abstractions.Data;

namespace TCTOS.Impls.Local;

public sealed class LocalUnixSocketListener(string socketPath, ILogger<LocalUnixSocketListener> logger)
    : ISocketListener, IDisposable
{
    public event SocketEventHandler? OnMessage;

    private Socket? _socket;

    public async Task ListenAsync(CancellationToken cancellationToken)
    {
        logger.LogTrace("Creating new socket");
        _socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
        logger.LogTrace("Binding socket");
        logger.LogTrace("Socket path is {socketPath}", socketPath);
        _socket.Bind(new UnixDomainSocketEndPoint(socketPath));
        logger.LogTrace("Placing socket in listening state");
        _socket.Listen();
        File.SetUnixFileMode(socketPath,
            UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.GroupRead | UnixFileMode.GroupWrite);
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    logger.LogTrace("Waiting for connection");
                    var socket = await _socket.AcceptAsync(cancellationToken);
                    logger.LogTrace("Connection received");
                    var buffer = new byte[4096];
                    var numberOfBytesReceived = socket.Receive(buffer, 0, buffer.Length, SocketFlags.None);
                    logger.LogTrace("Received {bytes} bytes", numberOfBytesReceived);
                    var text = Encoding.UTF8.GetString(buffer[..numberOfBytesReceived]);
                    logger.LogTrace("Received text is {text}", text);

                    try
                    {
                        logger.LogTrace("Attempting to deserialize socket message");
                        var socketMessage = JsonSerializer.Deserialize<SocketMessage>(text)!;
                        logger.LogTrace("Invoking event");
                        var response = OnMessage?.Invoke(socketMessage);
                        logger.LogTrace("Response is {response}", response);
                        if (response != null)
                        {
                            logger.LogTrace("Response is not null");
                            var responseText = JsonSerializer.Serialize(response);
                            logger.LogTrace("Response text is {responseText}", responseText);
                            var responseBytes = Encoding.UTF8.GetBytes(responseText);
                            logger.LogTrace("Sending bytes");
                            await socket.SendAsync(responseBytes);
                        }
                    }
                    // TODO: Cleanup duplicate code
                    catch (JsonException)
                    {
                        logger.LogWarning("Failed to deserialize message");
                        var responseText = JsonSerializer.Serialize(new SocketResponse(error: "Invalid message"));
                        logger.LogTrace("Response text is {responseText}", responseText);
                        var responseBytes = Encoding.UTF8.GetBytes(responseText);
                        logger.LogTrace("Sending bytes");
                        await socket.SendAsync(responseBytes);
                    }
                    catch (InvalidOperationException)
                    {
                        logger.LogWarning("Failed to deserialize message");
                        var responseText = JsonSerializer.Serialize(new SocketResponse(error: "Invalid message"));
                        logger.LogTrace("Response text is {responseText}", responseText);
                        var responseBytes = Encoding.UTF8.GetBytes(responseText);
                        logger.LogTrace("Sending bytes");
                        await socket.SendAsync(responseBytes);
                    }

                    logger.LogTrace("Closing socket");
                    socket.Close();
                    logger.LogTrace("Socket closed");
                }
                catch (SocketException e)
                {
                    logger.LogWarning("Exception occured in socket communication: {error}", e);
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    public void Dispose()
    {
        logger.LogDebug("Disposing socket");
        logger.LogTrace("Disconnecting socket");
        _socket?.Disconnect(false);
        logger.LogTrace("Closing socket");
        _socket?.Close();
        logger.LogTrace("Disposing socket");
        _socket?.Dispose();
        logger.LogTrace("Deleting socket at {socketPath}", socketPath);
        File.Delete(socketPath);
    }
}