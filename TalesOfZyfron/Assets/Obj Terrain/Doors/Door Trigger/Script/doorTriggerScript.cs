using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorTriggerScript : MonoBehaviour
{
    public GameObject DoorLeft;
    public GameObject DoorRight;
    private Animator DoorTLController;
    private Animator DoorTRController;
    private bool CanOpen;

    void Start()
    {
        DoorTLController = DoorLeft.GetComponent<Animator>();
        DoorTRController = DoorRight.GetComponent<Animator>();
        CanOpen = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(CanOpen)
        {
            OpenDoor();
            CanOpen = false;
            Invoke(nameof(ResetOpen),2);
        }
    }

    private void ResetOpen()
    {
        CanOpen = true;
    }
    private void OpenDoor()
    {
            DoorTLController.SetBool("Open",!DoorTLController.GetBool("Open"));
            DoorTRController.SetBool("Open",!DoorTRController.GetBool("Open"));
    }
}

