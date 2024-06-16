using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
public class EscapeMenu : NetworkBehaviour
{
    private KeyCode escapeKey = KeyCode.None;
    [SerializeField] NewPlayerController pc;
    [SerializeField] private FirstPersonCamera fpc;
    [SerializeField] private GameObject escapeScreen;
    private KeybindManager keybind;

    bool isEscape;
    bool disconnect;
    // Start is called before the first frame update
    void Start()
    {
        disconnect = false;
        isEscape = true;
        escapeScreen.SetActive(false);

        keybind = GetComponent<KeybindManager>();
        escapeKey = keybind.escapeKey;


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(escapeKey))
        {
            UpdatePauseMenu();
        }
        if(disconnect)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            if (players.Length <= 1) 
            {
                GameObject eventSystem = GameObject.FindGameObjectWithTag("Event System");
                Destroy(eventSystem);
                NetworkManager.Singleton.Shutdown();
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
            }
            
        }

    }
    public void UpdatePauseMenu()
    {

        if (!isEscape)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;

        }
        fpc.enabled = !isEscape;
        pc.enabled = !isEscape;
        escapeScreen.SetActive(isEscape);
        isEscape = !isEscape;

    }
    public void Disconnect()
    {
        
        if(IsHost)
        {
            Debug.Log("Host");
            TestClientRpc();
            disconnect = true;
            
        }
        else if (IsClient)
        {
            GameObject eventSystem = GameObject.FindGameObjectWithTag("Event System");
            Destroy(eventSystem);
            NetworkManager.Singleton.Shutdown();
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);

        }
      


    }





    [ClientRpc]
    private void TestClientRpc()
    {
        if (!IsHost)
        {
            GameObject eventSystem = GameObject.FindGameObjectWithTag("Event System");
            Destroy(eventSystem);
            NetworkManager.Singleton.Shutdown();
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
          
        }
    }
}
