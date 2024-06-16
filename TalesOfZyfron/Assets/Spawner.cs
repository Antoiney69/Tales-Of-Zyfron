using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;



public class Spawner : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform spawnPoint;
    public GameObject enemy;
    public float startTimeBtwSpawns;
    float timeBtwSpawns;
    public Quaternion rotation;
    public int nbSpawn;

    private void Start()
    {

    }
    private void Update()
    {
        if (!NetworkManager.Singleton.IsServer || nbSpawn==0)
        {
            return; 
        }
        nbSpawn-=1;
        GameObject monster = Instantiate(enemy, spawnPoint.position, rotation) as GameObject;
        var onlineMonster = monster.GetComponent<NetworkObject>();
        onlineMonster.Spawn(true);
        if (nbSpawn<0)
        {
           Destroy(spawnPoint);
        }
        
    }
}
