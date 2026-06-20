using UnityEngine;

namespace LinkSlider.Client
{
    public class GameGrpcPingButton : MonoBehaviour
    {
        [SerializeField]
        private GrpcClient client;

        public void OnClickPing()
        {
            if (client == null)
            {
                Debug.LogError("GrpcClient is not assigned.");
                return;
            }

            _ = client.PingAsync("Hello from GameGrpcPingButton!");
        }
    }
}