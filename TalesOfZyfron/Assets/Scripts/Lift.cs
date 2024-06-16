using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class Lift : NetworkBehaviour
{
    private int playersInLift;
    public float range;
    private List<Transform> playersInRange = new List<Transform>();
    bool canChange = false;

    public LayerMask playerLayer;
    // Start is called before the first frame update
    void Start()
    {
        playersInLift = 0;
        canChange = true;

    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlayersInRange();
        GameManager gm = FindObjectOfType<GameManager>();
        

        if (gm != null )
        {

            if (gm.alivePlayers == playersInLift && gm.alivePlayers != 0 && canChange)
            {
                gm.LoadNextLevelClientRpc();
                canChange = false;

            }
               





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
        playersInLift = playersInRange.Count;

    }

}




