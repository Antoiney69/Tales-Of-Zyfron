using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class Key : NetworkBehaviour
{
    public GameObject door;
    public GameObject key;
    public TextMeshPro text;
    private List<Transform> playersInRange = new List<Transform>();

    public LayerMask playerLayer;
    private DoorSingle openDoorScript;
    private DoorDouble openDoubleDoorScript;

    void Update()
    {
        UpdatePlayersInRange();
    }

    private void UpdatePlayersInRange()
    {
            playersInRange.Clear();
            Collider[] colliders = Physics.OverlapSphere(transform.position, 10f, playerLayer);
            foreach (Collider col in colliders)
            {
                text.GetComponent<Transform>().LookAt(col.transform);
            }

    }

    [ClientRpc]
    public void DestroyClientRpc()
    {
        Destroy(key);
    }
    [ServerRpc(RequireOwnership=false)]
    public void DestroyServerRpc(){
        Destroy(key);
    }

    public void FindKey()
    {
        DoorSingle openDoorScript = door.GetComponent<DoorSingle>();

        DoorDouble openDoubleDoorScript = door.GetComponent<DoorDouble>();
        Debug.Log(openDoorScript);
        if(openDoorScript != null){
        
            openDoorScript.FindKey();
        }
        else
        {
            openDoubleDoorScript.FindKey();
        } 
        DestroyServerRpc();  
        DestroyClientRpc();
        Destroy(key);
    }
}
