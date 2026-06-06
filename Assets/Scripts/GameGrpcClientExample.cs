using System;
using System.Net.Http;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using UnityEngine;
using LinkSlider.Grpc;

public class GameGrpcClientExample : MonoBehaviour
{
    [SerializeField]
    private string serverAddress = "http://127.0.0.1:8080";

    private GrpcChannel channel;
    private GameService.GameServiceClient client;

    private async void Start()
    {
        await InitializeClientAsync();
    }

    private async Task InitializeClientAsync()
    {
        var candidateAddresses = GetCandidateServerAddresses(serverAddress);

        foreach (var candidateAddress in candidateAddresses)
        {
            var httpHandler = new HttpClientHandler
            {
                UseProxy = false
            };
            var grpcWebHandler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, httpHandler);
            var candidateChannel = GrpcChannel.ForAddress(candidateAddress, new GrpcChannelOptions
            {
                HttpHandler = grpcWebHandler
            });
            var candidateClient = new GameService.GameServiceClient(candidateChannel);

            try
            {
                await PingOnceAsync(candidateClient);

                channel?.Dispose();
                channel = candidateChannel;
                client = candidateClient;
                serverAddress = candidateAddress;
                return;
            }
            catch
            {
                // Ignore and try next candidate
                candidateChannel.Dispose();
                continue;
            }
        }

        Debug.LogError($"Failed to connect to gRPC server using any loopback address. Last configured address was {serverAddress}.");
    }

    private static string[] GetCandidateServerAddresses(string configuredAddress)
    {
        var alternateAddress = GetAlternateLoopbackAddress(configuredAddress);

        if (string.IsNullOrEmpty(alternateAddress) || string.Equals(alternateAddress, configuredAddress, StringComparison.OrdinalIgnoreCase))
        {
            return new[] { configuredAddress };
        }

        return new[] { configuredAddress, alternateAddress };
    }

    private static string GetAlternateLoopbackAddress(string address)
    {
        if (!Uri.TryCreate(address, UriKind.Absolute, out var uri))
        {
            return address;
        }

        var alternateHost = uri.Host switch
        {
            "localhost" => "127.0.0.1",
            "127.0.0.1" => "localhost",
            _ => null
        };

        if (alternateHost == null)
        {
            return address;
        }

        var builder = new UriBuilder(uri)
        {
            Host = alternateHost
        };

        return builder.Uri.ToString();
    }

    private async Task PingOnceAsync(GameService.GameServiceClient targetClient)
    {
        if (targetClient == null)
        {
            Debug.LogError("gRPC client is not initialized.");
            return;
        }

        var reply = await targetClient.PingAsync(new PingRequest
        {
            Message = "hello from Unity"
        });

        Debug.Log($"Ping response: {reply.Message}");
    }

    public async Task PingAsync()
    {
        if (client == null)
        {
            Debug.LogError("gRPC client is not initialized.");
            return;
        }
        try
        {
            await PingOnceAsync(client);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Ping failed: {ex.Message}");
        }
    }

    public async Task MovePlayerAsync(string playerId, int x, int y)
    {
        if (client == null)
        {
            Debug.LogError("gRPC client is not initialized.");
            return;
        }
        try
        {
            var reply = await client.MovePlayerAsync(new MovePlayerRequest
            {
                PlayerId = playerId,
                X = x,
                Y = y
            });

            Debug.Log($"MovePlayer response: ok={reply.Ok}, message={reply.Message}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"MovePlayer failed: {ex.Message}");
        }
    }

    private void OnDestroy()
    {
        channel?.Dispose();
    }
}
