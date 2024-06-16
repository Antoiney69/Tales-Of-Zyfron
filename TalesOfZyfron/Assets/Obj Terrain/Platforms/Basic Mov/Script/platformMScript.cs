using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMS : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject.Find("Player"));
    }

    private void OnTriggerExit(Collider other)
    {
        //other.transform.parent = null;
    }
}
