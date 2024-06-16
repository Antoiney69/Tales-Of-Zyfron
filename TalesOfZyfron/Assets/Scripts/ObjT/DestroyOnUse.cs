using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DestroyOnUse : MonoBehaviour
{
    public GameObject Door;
    public GameObject Text;
    private Animator DoorBController;

    void Start()
    {
        DoorBController = Door.GetComponent<Animator>();
    }

    void Update(){

    if(DoorBController.GetBool("Open"))
    {
        Destroy(Text);
        Destroy(this);
    }
    }

}
