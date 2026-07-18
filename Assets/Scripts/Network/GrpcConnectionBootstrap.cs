using System;
using System.Net.Http;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;

namespace LinkSlider.Client
{
    public static class GrpcConnectionBootstrap
    {
        public static GrpcChannel CreateChannel(string serverAddress)
        {
            var httpHandler = new HttpClientHandler
            {
                UseProxy = false
            };

            var grpcWebHandler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, httpHandler);

            return GrpcChannel.ForAddress(serverAddress, new GrpcChannelOptions
            {
                HttpHandler = grpcWebHandler
            });
        }

        public static string[] GetCandidateServerAddresses(string configuredAddress)
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
    }
}