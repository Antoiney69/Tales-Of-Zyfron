using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomObjectSpawner : MonoBehaviour
{
    public GameObject objectToSpawn;  // Le prefab de l'objet à instancier
    public float spawnInterval = 1f;  // Intervalle de temps entre chaque spawn
    
    public List<Transform> spawnPoints;  // Liste des points de spawn
    public float objectSpeed = 2f;  // Vitesse de déplacement des objets
    public float objectLifetime = 5f;  // Durée de vie de l'objet en secondes

    void Start()
    {
        if (spawnPoints.Count == 0)
        {
            Debug.LogError("Aucun point de spawn défini !");
            return;
        }
        StartCoroutine(SpawnObjects());
    }

    IEnumerator SpawnObjects()
    {
        while (true)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
            GameObject newObject = Instantiate(objectToSpawn, spawnPoint.position, spawnPoint.rotation);
            
            StartCoroutine(MoveObject(newObject));
            DestroyAfterTime(newObject, objectLifetime);

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    IEnumerator MoveObject(GameObject obj)
    {
        yield return new WaitForSeconds(Random.Range(0f, 2f));  // Attendre un moment random entre 0 et 2 secondes

        while (obj != null)
        {
            obj.transform.Translate(Vector3.forward * objectSpeed * Time.deltaTime);
            yield return null;
        }
    }

    void DestroyAfterTime(GameObject obj, float lifetime)
    {
        Destroy(obj, lifetime);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        foreach (var spawnPoint in spawnPoints)
        {
            if (spawnPoint != null)
                Gizmos.DrawWireSphere(spawnPoint.position, 0.5f);
        }
    }
}