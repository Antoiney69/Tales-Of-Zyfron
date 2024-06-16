using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class DoorSingle : NetworkBehaviour
{
    private float resetOpenCooldown = 0.1f;
    public TextMeshPro Text;  
    public GameObject door;
    public bool oneOpen = false;
    public bool needKey = false;
    public AudioSource source;
    public AudioClip open;
    public AudioClip close;
    public AudioClip locke;
    private Animator doorEController;
    private bool canOpen;
    void Start()
    {
        doorEController = door.GetComponent<Animator>();
        if(needKey)
            canOpen = false;
        else
            canOpen = true;
    }

    private void ResetOpen()
    {
        canOpen = true;
    }


    [ServerRpc(RequireOwnership = false)]
    public void OpenDoorServerRpc()
    {
        if (canOpen)
        {
            Debug.Log(doorEController.GetBool("Open"));

            doorEController.SetBool("Open", !doorEController.GetBool("Open"));
            canOpen = false;
            Invoke(nameof(ResetOpen), resetOpenCooldown);
            if(!doorEController.GetBool("Open"))
            {
                source.PlayOneShot(open);
            }
            else
            {
                source.PlayOneShot(close);
            }
            if(oneOpen)
            {
                Destroy(this);
            }
        }
        else
            Text.text = "Need a key";
            source.PlayOneShot(locke);
    }

    public void FindKey()
    {
        Debug.Log("You have found the key");
        Text.text = "[E]";
        canOpen = true;
    }
    // Client-side method to call the server RPC
    public void OpenDoor()
    {
        OpenDoorServerRpc();
    }
      
}

