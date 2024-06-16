using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class HeadRotation : NetworkBehaviour
{
    public Transform playerCamera; // Référence à la caméra du joueur
    public Transform head;         // Référence à la tête du joueur

    public float minVerticalAngle = -45.0f; // Angle minimum de la tête (regarder en bas)
    public float maxVerticalAngle = 45.0f;  // Angle maximum de la tête (regarder en haut)

    // NetworkVariable to synchronize head rotation
    private NetworkVariable<Vector2> networkHeadRotation = new NetworkVariable<Vector2>(
        new Vector2(0, 0),
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    void LateUpdate()
    {
        if (IsOwner)
        {
            // Obtenir les rotations actuelles de la caméra sur les axes X et Y
            float cameraRotationX = playerCamera.eulerAngles.x;

            // Convertir l'angle de rotation X de la caméra de 0-360 à -180 à 180
            if (cameraRotationX > 180)
            {
                cameraRotationX -= 360;
            }

            // Limiter l'angle de rotation X
            cameraRotationX = Mathf.Clamp(cameraRotationX, minVerticalAngle, maxVerticalAngle);

            // Update the head rotation locally
            Quaternion headRotation = Quaternion.Euler(cameraRotationX, head.rotation.eulerAngles.y, head.rotation.eulerAngles.z);
            head.rotation = headRotation;

            // Update the network variable
            networkHeadRotation.Value = new Vector2(cameraRotationX, head.rotation.eulerAngles.y);
        }
        else
        {
            // Apply the network head rotation to the head transform
            Vector2 networkRotation = networkHeadRotation.Value;
            Quaternion headRotation = Quaternion.Euler(networkRotation.x, networkRotation.y, head.rotation.eulerAngles.z);
            head.rotation = headRotation;
        }
    }
}