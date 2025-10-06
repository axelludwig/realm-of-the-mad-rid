using UnityEngine;
using UnityEngine.InputSystem;

public class UIInputManager : MonoBehaviour
{
    [SerializeField] private GameObject debugMenu;
    InputAction f3action;
    bool isActive = false;

    private void Awake()
    {
        f3action = InputSystem.actions.FindAction("DebugMenu");
    }

    private void Update()
    {
        if(f3action.WasPressedThisFrame())
        {
            isActive = !isActive;
            debugMenu.SetActive(isActive);
        }
    }

}
