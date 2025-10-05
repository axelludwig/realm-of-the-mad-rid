using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    private Vector2 v_MoveInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
