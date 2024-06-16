using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatfromFS : MonoBehaviour
{
    public GameObject Platform;
    private Animator PlatformFController;

    void Start()
    {
        PlatformFController = Platform.GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Cube")
        {
            Debug.Log(PlatformFController.GetBool("Fall"));
            PlatformFController.SetBool("Fall",true);
        }
    }
}
