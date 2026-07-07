using UnityEngine;

namespace LinkSlider.Client
{
    public class GameGrpcPingButton : MonoBehaviour
    {
        [SerializeField]
        private GrpcClient client;

        [SerializeField]
        private string message = "Hello from GameGrpcPingButton!";

        public void OnClickPing()
        {
            if (client == null)
            {
                Debug.LogError("GrpcClient is not assigned.");
                return;
            }

            _ = client.PingAsync(message);
        }
    }
}