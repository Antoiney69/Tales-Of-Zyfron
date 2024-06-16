using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class RotationCamera : NetworkBehaviour
{
    // Start is called before the first frame update
    public Transform cameraTransform;

    // Vitesse de rotation du joueur
    public float rotationSpeed = 5.0f;

    void Update()
    {
        // Vérifie si la caméra est définie
        if (cameraTransform != null)
        {
            // Obtenir la direction vers laquelle la caméra regarde, mais en ignorant la composante y
            Vector3 cameraForward = cameraTransform.forward;
            cameraForward.y = 0.0f;

            // Si la direction n'est pas nulle
            if (cameraForward != Vector3.zero)
            {
                // Calculer la rotation cible
                Quaternion targetRotation = Quaternion.LookRotation(cameraForward);

                // Interpoler la rotation actuelle vers la rotation cible
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }
}
