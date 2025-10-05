using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    private Vector2 v_MoveInput;
    [SerializeField] private GameObject ProjectilePrefab;

    private Camera Camera;

    private void Start()
    {
        if (IsOwner)
            Camera = Camera.main;
    }

    /// <summary>
    /// Détecte le clic gauche pour tirer.
    /// </summary>
    public void OnAttack(InputAction.CallbackContext p_Context)
    {
        if (!IsOwner || !p_Context.performed) return;

        // Calcul direction souris
        Vector3 v_MouseWorld = Camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 v_Direction = (v_MouseWorld - transform.position).normalized;

        // Demande au serveur d’instancier le projectile
        SpawnProjectileServerRpc(transform.position, v_Direction);
    }

    [ServerRpc]
    private void SpawnProjectileServerRpc(Vector3 p_Position, Vector2 p_Direction)
    {
        GameObject v_Projectile = Instantiate(ProjectilePrefab, p_Position, Quaternion.identity);
        v_Projectile.GetComponent<Projectile>().Initialize(p_Direction);
        v_Projectile.GetComponent<NetworkObject>().Spawn();
    }

    /// <summary>
    /// Appelée automatiquement par le Input System quand l'action "Move" est utilisée.
    /// </summary>
    /// <param name="p_Context">Contexte d'entrée contenant les valeurs de mouvement</param>
    public void OnMove(InputAction.CallbackContext p_Context)
    {
        v_MoveInput = p_Context.ReadValue<Vector2>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        Vector3 v_Movement = new Vector3(v_MoveInput.x, v_MoveInput.y, 0f);
        transform.position += v_Movement * 5 * Time.deltaTime;
    }
}
