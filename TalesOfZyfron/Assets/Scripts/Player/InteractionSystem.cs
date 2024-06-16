using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using Unity.VisualScripting;
using Unity.Netcode;
using UnityEngine;
using System;

public class InteractionSystem : NetworkBehaviour
{
    

    [Header("Combat")]
    [SerializeField] private Camera camera;
    [SerializeField] private float range = 3f;
   
    private GameObject aimedInteractableObject;

    public LayerMask playerLayer;
    private KeybindManager keybind;
    private KeyCode interactionKey = KeyCode.None;

    private void Start()
    {
        keybind = GetComponent<KeybindManager>();
        interactionKey = keybind.interactionKey;
    }





    private void Update()
    {
        if (!IsOwner) return;
        if (Input.GetKeyDown(interactionKey))
            Interact();
       
        HandleInteraction();
    }
    private void HandleInteraction()

    {
        Ray ray = new Ray(camera.transform.position, camera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit raycastHitInfo, range,~playerLayer))
        {
            if (raycastHitInfo.collider != null)
            {
                aimedInteractableObject = raycastHitInfo.collider.gameObject;
            }
            else
            {
                aimedInteractableObject = null;
            }
        }
        else
        {
            aimedInteractableObject = null;
        }
    }


    private void Interact()
    {
        if(aimedInteractableObject != null)
        {
            Debug.Log(aimedInteractableObject);
            DoorSingle openDoorScript = aimedInteractableObject.GetComponentInParent<DoorSingle>();

            DoorDouble openDoubleDoorScript = aimedInteractableObject.GetComponentInParent<DoorDouble>();

            Key findKeyScript = aimedInteractableObject.GetComponentInParent<Key>();

            EnemyHp enemyHp = aimedInteractableObject.GetComponentInParent<EnemyHp>();

            StartButton start = aimedInteractableObject.GetComponent<StartButton>();

            // If the script component is found, call its Open method
            if (findKeyScript != null)
            {
                findKeyScript.FindKey();
            }
            if (openDoorScript != null)
            {
                openDoorScript.OpenDoor();
            }
            if (openDoubleDoorScript != null)
            {
                openDoubleDoorScript.OpenDoor();
            }
            if (enemyHp != null)
            {
                enemyHp.TakeDamage(1);
            }
            if (start != null)
            {
                start.StarGame();
            }

        }
      
    }
}
