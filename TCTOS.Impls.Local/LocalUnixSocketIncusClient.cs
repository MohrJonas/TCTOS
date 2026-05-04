using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.Json.Serialization;
using TCTOS.Abstractions;
using TCTOS.Abstractions.Incus.DTOs;
using TCTOS.Abstractions.Incus.Response;

namespace TCTOS.Impls.Local;

public sealed class LocalUnixSocketIncusClient : IIncusClient
{
    private readonly HttpClient _httpClient = new(new SocketsHttpHandler
    {
        ConnectCallback = async (_, _) =>
        {
            var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);
            var endpoint = new UnixDomainSocketEndPoint("/var/lib/incus/unix.socket");
            await socket.ConnectAsync(endpoint);
            return new NetworkStream(socket, true);
        }
    });

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };


    public Task<ResponseBase> StartContainerAsync(string containerName)
    {
        return DoHttpRequest(
            $"/1.0/instances/{containerName}/state",
            HttpMethod.Put,
            MediaTypeNames.Application.Json,
            new InstanceStatePut
            {
                Action = "start",
                Force = false,
                Stateful = false,
                Timeout = 120
            }
        );
    }

    public Task<ResponseBase> StopContainerAsync(string containerName)
    {
        return DoHttpRequest(
            $"/1.0/instances/{containerName}/state",
            HttpMethod.Put,
            MediaTypeNames.Application.Json,
            new InstanceStatePut
            {
                Action = "stop",
                Force = false,
                Stateful = false,
                Timeout = 120
            }
        );
    }

    public Task<ResponseBase<Instance[]>> GetContainersAsync()
    {
        return DoHttpRequest<Instance[]>("/1.0/instances?recursion=1");
    }

    public Task<ResponseBase<string[]>> GetContainerNamesAsync()
    {
        return DoHttpRequest<string[]>("/1.0/instances");
    }

    public Task<ResponseBase<Image[]>> GetImagesAsync()
    {
        return DoHttpRequest<Image[]>("/1.0/images?recursion=1");
    }

    public Task<ResponseBase> UpdateContainerPartiallyAsync(string containerName, InstancePut changes)
    {
        return DoHttpRequest(
            $"/1.0/instances/{containerName}",
            HttpMethod.Patch,
            MediaTypeNames.Application.Json,
            changes
        );
    }

    public Task<ResponseBase> UpdateContainerAsync(string containerName, InstancePut changes)
    {
        return DoHttpRequest(
            $"/1.0/instances/{containerName}",
            HttpMethod.Put,
            MediaTypeNames.Application.Json,
            changes
        );
    }

    public Task<ResponseBase> CreateContainerAsync(InstancesPost containerParameters)
    {
        return DoHttpRequest(
            "/1.0/instances",
            HttpMethod.Post,
            MediaTypeNames.Application.Json,
            containerParameters
        );
    }

    public Task<ResponseBase<Instance>> GetContainerAsync(string containerName)
    {
        return DoHttpRequest<Instance>($"/1.0/instances/{containerName}");
    }

    public Task<ResponseBase<Image>> GetImageAsync(string fingerprint)
    {
        return DoHttpRequest<Image>($"/1.0/images/{fingerprint}");
    }

    public Task<ResponseBase> WaitForOperationAsync(string operationUrl)
    {
        return DoHttpRequest($"{operationUrl}/wait");
    }

    public Task<ResponseBase> RunCommand(string containerName, string command, string[]? args = null,
        Dictionary<string, object>? env = null, string? cwd = null)
    {
        return DoHttpRequest($"/1.0/instances/{containerName}/exec", HttpMethod.Post, MediaTypeNames.Application.Json,
            new InstanceExecPost
            {
                Command = [command, ..(args ?? [])],
                Environment = env?.Select(pair => KeyValuePair.Create(pair.Key, pair.Value.ToString()!)).ToDictionary(),
                Cwd = "/",
                Group = 0,
                User = 0
            });
    }

    public Task<ResponseBase> DeleteContainerAsync(string containerName)
        => DoHttpRequest($"/1.0/instance/{containerName}", HttpMethod.Delete);

    private Uri CreateUriFromPath(string path)
    {
        return new Uri($"http://ignored{path}");
    }

    private HttpRequestMessage CreateRequestMessage(
        string path,
        HttpMethod? method = null,
        string? contentType = null,
        object? body = null
    )
    {
        var request = new HttpRequestMessage
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
        return request;
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
}