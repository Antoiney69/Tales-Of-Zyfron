using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class EventSystemReload : NetworkBehaviour
{

    // Start is called before the first frame update
    private void Awake()
    {
        
        DontDestroyOnLoad(gameObject);
    }
}
