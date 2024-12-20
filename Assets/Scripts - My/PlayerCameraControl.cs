using Unity.Netcode;
using UnityEngine;

public class PlayerCameraControl : NetworkBehaviour
{
    private Camera playerCamera;
    private AudioListener audioListener;
    private void Start()
    {
        playerCamera = GetComponentInChildren<Camera>(); // Assuming the camera is a child of the player
        audioListener = GetComponent<AudioListener>();

        if (playerCamera == null) return;
        
        if (IsLocalPlayer) // Only enable the camera for the local player
        {
            playerCamera.enabled = true;
            audioListener.enabled = true;
        }
        else
        {
            playerCamera.enabled = false; // Disable camera for other players
            audioListener.enabled = false;
        }
    }
}