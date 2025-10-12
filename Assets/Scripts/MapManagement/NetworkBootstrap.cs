using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class NetworkBootstrap : MonoBehaviour
{
    IEnumerator Start()
    {
        // Attends que le NetworkManager soit prêt
        yield return new WaitUntil(() => NetworkManager.Singleton != null);

#if UNITY_EDITOR
        NetworkManager.Singleton.StartHost();
#else
        NetworkManager.Singleton.StartClient();
#endif
    }
}
