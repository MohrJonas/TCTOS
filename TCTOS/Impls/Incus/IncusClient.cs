using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using TCTOS.Abstractions;
using TCTOS.Impls.Incus.DTOs;
using TCTOS.Impls.Incus.Response;

namespace TCTOS.Impls.Incus;

public sealed class IncusClient(ILogger<IncusClient> logger) : IIncusClient
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
    
    private readonly HttpClient _httpClient = new(new SocketsHttpHandler
    {
        ConnectCallback = async (_, _) =>
        {
            var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);
            var endpoint = new UnixDomainSocketEndPoint("/var/lib/incus/unix.socket");
            await socket.ConnectAsync(endpoint);
            return new NetworkStream(socket, ownsSocket: true);
        }
    });
    
    private Uri CreateUriFromPath(string path) => new Uri($"http://ignored{path}");
    
    private HttpRequestMessage CreateRequestMessage(
        string path,
        HttpMethod? method = null,
        string? contentType = null,
        object? body = null
    )
    {
        var request = new HttpRequestMessage()
        {
            RequestUri = CreateUriFromPath(path),
            Method = method ?? HttpMethod.Get,
            Version = HttpVersion.Version11
        };
        if (body != null)
            request.Content = JsonContent.Create(
                body!,
                new MediaTypeHeaderValue(contentType ?? MediaTypeNames.Application.Json),
                _jsonSerializerOptions
            );
        var serialized = JsonSerializer.Serialize(body!, _jsonSerializerOptions);
;        return request;
    }

    private async Task<ResponseBase> DoHttpRequest(
        string path,
        HttpMethod? method = null,
        string? contentType = null,
        object? body = null)
    {
        var request = CreateRequestMessage(path, method, contentType, body);
        var response = await _httpClient.SendAsync(request);
        return (await response.Content.ReadFromJsonAsync<ResponseBase>())!;
    }

    private async Task<ResponseBase<TData>> DoHttpRequest<TData>(
        string path,
        HttpMethod? method = null,
        string? contentType = null,
        object? body = null
    )
    {
        var request = CreateRequestMessage(path, method, contentType, body);
        var response = await _httpClient.SendAsync(request);
        return (await response.Content.ReadFromJsonAsync<ResponseBase<TData>>())!;
    }


    public Task<ResponseBase> StartContainerAsync(string containerName)
        => DoHttpRequest(
            $"/1.0/instances/{containerName}/state",
            HttpMethod.Put,
            MediaTypeNames.Application.Json,
            new InstanceStatePut()
            {
                Action = "start",
                Force = false,
                Stateful = false,
                Timeout = 120
            }
        );

    public Task<ResponseBase> StopContainerAsync(string containerName)
        => DoHttpRequest(
            $"/1.0/instances/{containerName}/state",
            HttpMethod.Put,
            MediaTypeNames.Application.Json,
            new InstanceStatePut()
            {
                Action = "stop",
                Force = false,
                Stateful = false,
                Timeout = 120
            }
        );

    public Task<ResponseBase<Instance[]>> GetContainersAsync()
        => DoHttpRequest<Instance[]>("/1.0/instances?recursion=1");

    public Task<ResponseBase<Image[]>> GetImagesAsync()
        => DoHttpRequest<Image[]>("/1.0/images?recursion=1");

    public Task<ResponseBase> UpdateContainerAsync(string containerName, InstancesPost changes)
        => DoHttpRequest(
            $"/1.0/instances/{containerName}",
            HttpMethod.Put,
            MediaTypeNames.Application.Json,
            changes
        );

    public Task<ResponseBase> CreateContainerAsync(InstancesPost containerParameters)
        => DoHttpRequest(
            $"/1.0/instances",
            HttpMethod.Post,
            MediaTypeNames.Application.Json,
            containerParameters
        );

    public Task<ResponseBase<Instance>> GetContainerAsync(string containerName)
        => DoHttpRequest<Instance>($"/1.0/instances/{containerName}");

    public Task<ResponseBase<Image>> GetImageAsync(string fingerprint)
        => DoHttpRequest<Image>($"/1.0/images/{fingerprint}");

    public Task<ResponseBase> WaitForOperationAsync(string operationUrl)
        => DoHttpRequest($"{operationUrl}/wait");
}