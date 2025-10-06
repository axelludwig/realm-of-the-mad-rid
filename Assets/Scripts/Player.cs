using Unity.Netcode;
using UnityEngine;

public class Player: Entity
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            GameManager.instance.RegisterPlayer(this);
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (IsServer)
        {
            GameManager.instance.UnregisterPlayer(this);
        }
    }


}
