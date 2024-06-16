using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head : MonoBehaviour
{
    // Les angles de rotation minimum et maximum sur l'axe Y
    public float minYRotation = -45f;
    public float maxYRotation = 45f;

    // Référence au Transform de la tête du joueur
    public Transform headTransform;

    void Update()
    {
        // Obtenir la rotation actuelle de la tête
        Vector3 currentRotation = headTransform.localEulerAngles;

        // La rotation en Y peut être supérieure à 180 degrés, donc nous la remettons dans l'intervalle [-180, 180]
        if (currentRotation.y > 180f)
        {
            currentRotation.y -= 360f;
        }

        // Limiter la rotation sur l'axe Y
        currentRotation.y = Mathf.Clamp(currentRotation.y, minYRotation, maxYRotation);

        // Appliquer la rotation limitée à la tête du joueur
        headTransform.localEulerAngles = currentRotation;
    }
}
