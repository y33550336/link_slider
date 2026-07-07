using Grpc.Net.Client;
using LinkSlider.Grpc;

namespace LinkSlider.Client
{
    public static class GameGrpcClientFactory
    {
        public static GameService.GameServiceClient CreateClient(GrpcChannel channel)
        {
            return new GameService.GameServiceClient(channel);
        }
    }
}