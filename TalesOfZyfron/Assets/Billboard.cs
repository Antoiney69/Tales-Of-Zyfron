using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Billboard : NetworkBehaviour
{
    public Transform cam;

    private void LateUpdate()
    {
        transform.LookAt(transform.position+ cam.forward);
    }
}
