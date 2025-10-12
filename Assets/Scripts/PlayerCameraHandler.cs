using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerCameraHandler : NetworkBehaviour
{
    private Camera playerCamera;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10f);

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        // Crée la caméra du joueur local
        GameObject cameraGO = new GameObject("PlayerCamera");
        playerCamera = cameraGO.AddComponent<Camera>();
        playerCamera.orthographic = true;           // Vue 2D
        playerCamera.orthographicSize = 5f;         // ajuste la taille si besoin
        cameraGO.tag = "MainCamera";                // nécessaire pour UI/raycast

        var pixelPerfectCamera = cameraGO.AddComponent<PixelPerfectCamera>();
        pixelPerfectCamera.assetsPPU = 16;

        // Position initiale
        cameraGO.transform.position = transform.position + offset;
    }

    private void LateUpdate()
    {
        if (!IsOwner || playerCamera == null) return;

        // Caméra suit directement le joueur
        playerCamera.transform.position = transform.position + offset;
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner && playerCamera != null)
        {
            Destroy(playerCamera.gameObject);
        }
    }
}