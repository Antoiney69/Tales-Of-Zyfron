using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using Unity.Services.Relay;
public class StartButton : NetworkBehaviour
{



    public void StarGame()
    {

        if (IsServer)
        {
            GameManager gm = FindObjectOfType<GameManager>();
            gm.LoadSceneClientRpc("Level 1");
            GameManager gms = gm.GetComponent<GameManager>();
            gms.gameStarted.Value = true;
        }


    }



}
