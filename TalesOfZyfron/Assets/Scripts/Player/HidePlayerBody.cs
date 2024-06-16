using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerSetup : NetworkBehaviour
{
    public Camera playerCamera;

    private void Start()
    {
        if (IsLocalPlayer)
        {
            // Ensure the player's own body is on the PlayerBody layer
            SetLayerRecursively(transform, LayerMask.NameToLayer("PlayerBody"));

            // Hide the player's own body by adjusting the camera's culling mask
            playerCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("PlayerBody"));
        }
        else
        {
            // Ensure other players' bodies are not on the PlayerBody layer
            SetLayerRecursively(transform, 0); // Default layer (0)
        }
    }

    private void SetLayerRecursively(Transform obj, int newLayer)
    {
        obj.gameObject.layer = newLayer;
        foreach (Transform child in obj)
        {
            SetLayerRecursively(child, newLayer);
        }
    }
}
