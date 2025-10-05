using Unity.Netcode;
using UnityEngine;

public class NetworkStartUI : MonoBehaviour
{
    private void OnGUI()
    {
        float w = 200f, h = 40f;
        float x = 10f, y = 10f;

        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            if (GUI.Button(new Rect(x, y, w, h), "Host")) NetworkManager.Singleton.StartHost();
            y += h + 10;
            if (GUI.Button(new Rect(x, y, w, h), "Client")) NetworkManager.Singleton.StartClient();
            y += h + 10;
            if (GUI.Button(new Rect(x, y, w, h), "Server")) NetworkManager.Singleton.StartServer();
        }
    }
}
