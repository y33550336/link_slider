using System.Threading.Tasks;
using LinkSlider.Grpc;
using UnityEngine;

namespace LinkSlider.Client
{
    public static class GameGrpcPingService
    {
        public static async Task PingAsync(GameService.GameServiceClient client, string message = "hello from Unity")
        {
            if (client == null)
            {
                Debug.LogError("gRPC client is not initialized.");
                throw new System.InvalidOperationException("gRPC client is not initialized.");
            }

            var reply = await client.PingAsync(new PingRequest
            {
                Message = message
            });

            Debug.Log($"Ping response: {reply.Message}");
        }
    }
}