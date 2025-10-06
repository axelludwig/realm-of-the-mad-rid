using Unity.Netcode;
using UnityEngine;

public class GameManager : BaseSingleton<GameManager>
{
    [SerializeField] private GameObject v_EnemyPrefab;

    /// <summary>
    /// Fait apparaître un ennemi sur le serveur à une position donnée.
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    public void SpawnEnemyServerRpc(Vector2 p_Pos)
    {
        GameObject v_Enemy = Instantiate(v_EnemyPrefab, p_Pos, Quaternion.identity);
        v_Enemy.GetComponent<NetworkObject>().Spawn(true);
    }
}
