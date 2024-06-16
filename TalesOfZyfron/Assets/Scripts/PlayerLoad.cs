using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerLoad : NetworkBehaviour
{
    public GameObject playerPrefab;
    // Start is called before the first frame update
    void Start()
    {
        if (!IsOwner) return;
        GameObject player = Instantiate(playerPrefab);


    }

    // Update is called once per frame
   
}
