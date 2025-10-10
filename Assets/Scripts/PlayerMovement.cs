using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    private Vector2 v_MoveInput;
    private Entity entity;
    [SerializeField] private GameObject ProjectilePrefab;
    [SerializeField] private GameObject EnemyPrefab;

    private float lastAttackTime;
    private bool isAttacking;

    private Camera Camera;

    private void Start()
    {
        entity = GetComponent<Entity>();
        if (IsOwner)
            Camera = Camera.main;
    }

    private void Update()
    {
        if (!IsOwner) return;
        HandleMovement();

        if (!isAttacking) return;
        HandleAttackHold();
    }

    /// <summary>
    /// Détecte le clic gauche pour tirer.
    /// </summary>
    public void OnAttack(InputAction.CallbackContext p_Context)
    {
        if (!IsOwner || !Camera) return;

        if(p_Context.started)
        {
            isAttacking = true;
        } 
        else if(p_Context.canceled)
        {
            isAttacking = false;
        }
    }

    private void HandleAttackHold()
    {
        var attackCooldown = 1 / entity.Stats.AttackSpeed.CurrentValue;
        if (Time.time - lastAttackTime < attackCooldown) return;

        lastAttackTime = Time.time;

        Vector3 v_MouseWorld = Camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 v_Direction = (v_MouseWorld - transform.position).normalized;

        SpawnProjectileServerRpc(transform.position, v_Direction);
    }

    private void HandleMovement()
    {
        Vector3 v_Movement = new Vector3(v_MoveInput.x, v_MoveInput.y, 0f);
        transform.position += v_Movement * entity.Stats.MovementSpeed.CurrentValue * Time.deltaTime;
    }

    /// <summary>
    /// Détecte la touche R .
    /// </summary>
    public void OnSpawnEnemy(InputAction.CallbackContext p_Context)
    {
        if (!IsOwner || !p_Context.performed) return;

        // Calcul direction souris
        Vector3 v_MouseWorld = Camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        v_MouseWorld.z = 0;

        GameManager.Instance.SpawnEnemyServerRpc(v_MouseWorld);
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
}
