using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System.Linq;
using Unity.Services.Lobbies.Models;
using TMPro;

public class GameManager : NetworkBehaviour
{
    public NetworkVariable<bool> gameStarted = new NetworkVariable<bool>();
    [SerializeField] int maxLevelIndex;
    public string joinCode;
    private int randomLevelIndex;
    int levelCounter = 0;
    public int difficulty = 1;
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }


    void Awake()
    {
        gameStarted.Value = false;
        // Check if an instance already exists
        if (_instance != null && _instance != this)
        {
            // If an instance already exists, destroy this GameObject
            Destroy(gameObject);
            return;
        }

        // Set the instance
        _instance = this;
        DontDestroyOnLoad(gameObject);
        levelCounter = 0;
        difficulty = 1;
    }
    // Start is called before the first frame update
    public int alivePlayers = 1;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (IsServer)
        {
            ResetPlayerPositionsClientRpc();
        }
        if (scene.name == "HubLevel")
        {
            gameStarted.Value = false;
            TextMeshPro Text = GameObject.FindGameObjectWithTag("Code").GetComponent<TextMeshPro>();
            Text.text = "Le code de la partie est:\n" + joinCode;
        }
                 
    }



    [ClientRpc]
    public void ResetPlayerPositionsClientRpc()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            StartCoroutine(ResetPlayerPositionCoroutine(player));
        }
        alivePlayers = players.Length;
        Debug.Log($"Number of alive players: {alivePlayers}");
    }

    private IEnumerator ResetPlayerPositionCoroutine(GameObject player)
    {
        // Disable all MonoBehaviour scripts
        MonoBehaviour[] scripts = player.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            script.enabled = false;
        }

        // Disable all NetworkBehaviour scripts
        NetworkBehaviour[] networkScripts = player.GetComponents<NetworkBehaviour>();
        foreach (NetworkBehaviour script in networkScripts)
        {
            script.enabled = false;
        }

        // Set Rigidbody to kinematic
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        // Reset position on the server and all clients
        player.transform.position = new Vector3(0, 1, 0);
        UpdatePlayerPositionClientRpc(player.GetComponent<NetworkObject>().NetworkObjectId, player.transform.position);

        // Wait for a frame to ensure position is updated
        yield return null;

        // Enable all MonoBehaviour scripts
        foreach (MonoBehaviour script in scripts)
        {
            script.enabled = true;
        }

        // Enable all NetworkBehaviour scripts
        foreach (NetworkBehaviour script in networkScripts)
        {
            script.enabled = true;
        }

        // Unset Rigidbody kinematic
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        // Disable loading screen
        NewPlayerController pc = player.GetComponent<NewPlayerController>();
        if (pc != null)
        {
            pc.SetLoadingScreen(false);
        }

        // Confirm position for debugging
        Debug.Log($"Player {player.name} position reset to: {player.transform.position}");
    }
  


    [ClientRpc]
    private void UpdatePlayerPositionClientRpc(ulong networkObjectId, Vector3 position)
    {
        NetworkObject networkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];
        if (networkObject != null)
        {
            networkObject.transform.position = position;
        }
    }
    private void IncreaseEnemiesDifficulty()
    {
        Debug.Log("Applying function");
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Ennemy");
        foreach (GameObject enemy in enemies)
        {
            RangedEnemyAI enemyController = enemy.GetComponent<RangedEnemyAI>();
            if (enemyController != null)
            {
                enemyController.IncreaseDifficultyClientRpc(difficulty-1);
                Debug.Log("Applied on ennemy !");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            if (alivePlayers <= 0)
            {
                RestartGameClientRpc();
            }
                

        }
        

        


    }
    [ClientRpc]
    public void LoadSceneClientRpc(string sceneName)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {

            NewPlayerController playerController = player.GetComponent<NewPlayerController>();
            if (playerController != null)
            {
                playerController.SetLoadingScreen(true);
                playerController.enabled = false;

            }
        }
        if (IsServer)
        {
            NetworkManager.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }


    }


    [ClientRpc]
    public void LoadNextLevelClientRpc()
    {
        DifficultyScaler();
        levelCounter++;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {

            NewPlayerController playerController = player.GetComponent<NewPlayerController>();
            if (playerController != null)
            {
                playerController.SetLoadingScreen(true);


            }
        }
        
        randomLevelIndex = GetRandomLevelIndex();
        if (IsServer)
        {
            if (levelCounter % 5 == 0)
            {
                NetworkManager.SceneManager.LoadScene("Level Boss", LoadSceneMode.Single);

            }
            else
            {
                NetworkManager.SceneManager.LoadScene("Level " + randomLevelIndex, LoadSceneMode.Single);
            }
        }



    }
    public int GetRandomLevelIndex()
    {
        int newIndex;
        do
        {
            newIndex = Random.Range(2, maxLevelIndex);
        } while (newIndex == randomLevelIndex);

        randomLevelIndex = newIndex;
        return newIndex;
    }
    [ClientRpc]
    private void RestartGameClientRpc()
    {
        difficulty = 1;
        levelCounter = 0;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {

            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.SpawnPlayer();

            }
        }
        if (IsServer)
        {
            NetworkManager.SceneManager.LoadScene("HubLevel", LoadSceneMode.Single);

        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateAlvivePlayersServerRpc(int i)
    {
        alivePlayers += i;
    }
    private void DifficultyScaler()
    {
        difficulty++;

    }
    public void ExitGame()
    {
        Application.Quit();
    }
    
}
