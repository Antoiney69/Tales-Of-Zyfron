using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class DoorDouble : NetworkBehaviour
{
    private float resetOpenCooldown = 0.1f;
    public GameObject door;  
    public TextMeshPro text;  
    public bool oneOpen = false;
    public bool needKey = false;
    public AudioSource source;
    public AudioSource access;
    public AudioClip open;
    public AudioClip close;
    public AudioClip accessDenied;
    public AudioClip accessGranted;
    private Renderer renderer;
    private Animator DoorBController;
    private bool canOpen;

    void Start()
    {
        renderer = GetComponent<Renderer>();
        DoorBController = door.GetComponent<Animator>();
        if(needKey)
            canOpen = false;
        else
            canOpen = true;
    }


    private void ResetOpen()
    {
        canOpen = true;
    }

    private void ResetColor()
    {
        renderer.material.SetColor("_EmissionColor", Color.red);
    }

    [ServerRpc(RequireOwnership = false)]

    private void OpenDoorServerRpc()
    {
        Debug.Log(DoorBController.GetBool("Open"));
        if (canOpen)
        {
            DoorBController.SetBool("Open", !DoorBController.GetBool("Open"));
            canOpen = false;
            Invoke(nameof(ResetOpen), resetOpenCooldown);
            renderer.material.SetColor("_EmissionColor", Color.green);
            Invoke(nameof(ResetColor), 1.0f);
            access.PlayOneShot(accessGranted);
            if(oneOpen)
            {
                Destroy(this);
            }
            if(!DoorBController.GetBool("Open"))
            {
                source.PlayOneShot(open);
            }
            else
            {
                source.PlayOneShot(close);
            }
        }
        else
        {
            text.text = "Need a card";
            access.PlayOneShot(accessDenied);
        }
    }
    public void FindKey()
    {
        Debug.Log("You have found the key");
        text.text = "[E]";
        canOpen = true;
    }

    public void OpenDoor()
    {
        OpenDoorServerRpc();
    }

}
