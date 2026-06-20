using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using UnityEngine;
using LinkSlider.Grpc;
using LinkSlider.Client;

public class GrpcClient : MonoBehaviour
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
        var candidateAddresses = GrpcConnectionBootstrap.GetCandidateServerAddresses(serverAddress);

        foreach (var candidateAddress in candidateAddresses)
        {
            var candidateChannel = GrpcConnectionBootstrap.CreateChannel(candidateAddress);
            var candidateClient = GameGrpcClientFactory.CreateClient(candidateChannel);

            try
            {
                await GameGrpcPingService.PingAsync(candidateClient);

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

    public async Task PingAsync(string message)
    {
        if (client == null)
        {
            Debug.LogError("gRPC client is not initialized.");
            return;
        }
        try
        {
            await GameGrpcPingService.PingAsync(client, message);
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
