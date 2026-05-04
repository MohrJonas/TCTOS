using System.CommandLine;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;

namespace TCTOS.Console.Commands;

public sealed class EventsCommand(DiContainer container)
    : CommandBase("events", "Show received events", container)
{
    private readonly HttpMessageHandler _handler = new SocketsHttpHandler
    {
        ConnectCallback = async (_, _) =>
        {
            var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);
            var endpoint = new UnixDomainSocketEndPoint("/var/lib/incus/unix.socket");
            await socket.ConnectAsync(endpoint);
            return new NetworkStream(socket, true);
        }
    };

    protected override async Task RunAsync(ParseResult parseResult, DiContainer container, CancellationToken token)
    {
        //var client = container.Get<IIncusClient>();
        //var wsUrl = await client.GetEventWebsocketUrlAsync();

        var websocketClient = new ClientWebSocket();
        await websocketClient.ConnectAsync(new Uri("ws://ignored/1.0/events"), new HttpMessageInvoker(_handler), token);

        while (websocketClient.State == WebSocketState.Open)
        {
            var buffer = new byte[4096];
            var message = await websocketClient.ReceiveAsync(buffer, token);
            var messageString = Encoding.UTF8.GetString(buffer[..message.Count]);
            System.Console.WriteLine(messageString);
        }
    }
}