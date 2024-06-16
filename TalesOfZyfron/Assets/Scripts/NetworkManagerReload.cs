using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManagerReload : MonoBehaviour
{
    private static NetworkManagerReload _instance;
    public static NetworkManagerReload Instance { get { return _instance; } }
    // Start is called before the first frame update
    private void Awake()
    {
        // Check if an instance already exists
        if (_instance != null && _instance != this)
        {
            // If an instance already exists, destroy this GameObject
            Destroy(gameObject);
            return;
        }

        // Set the instance
        _instance = this;
    }
}
