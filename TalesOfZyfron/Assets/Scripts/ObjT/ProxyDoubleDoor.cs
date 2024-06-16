using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProxyDoubleDoor : NetworkBehaviour
{
    public float range;
    private List<Transform> playersInRange = new List<Transform>();
    public AudioSource source;
    public AudioClip open;
    public AudioClip close;
    public LayerMask playerLayer;
    private Animator DoorController;

    private void Start()
    {
  
        DoorController = GetComponent<Animator>();
        
    }
    // Update is called once per frame
    void Update()
    {
        UpdatePlayersInRange();
        if (playersInRange.Count == 0)
        {
            if(DoorController.GetBool("Open"))
            {
                source.PlayOneShot(close);
            }
            DoorController.SetBool("Open", false);
        }
        else
        {
            if(!DoorController.GetBool("Open"))
            {
                source.PlayOneShot(open);
            }
            DoorController.SetBool("Open", true);
        }


    }
    private void UpdatePlayersInRange()
    {
        playersInRange.Clear();
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, playerLayer);
        foreach (Collider col in colliders)
        {
            Transform playerTransform = col.transform;

            // Ensure the player is not already in the list
            if (!playersInRange.Contains(playerTransform))
            {
                playersInRange.Add(playerTransform);
            }

        }

    }
}
