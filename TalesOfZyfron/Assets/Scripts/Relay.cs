using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Relay : MonoBehaviour
{
  
    
    [SerializeField] TMP_InputField iF;
    private GameObject gameManager;
    private string joinCode;
    private async void Start()
    {
        await UnityServices.InitializeAsync();

        // Check if the player is already signed in
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            // If not signed in, sign in anonymously
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        }
        else
        {
            // Player is already signed in
            Debug.Log("Player is already signed in as: " + AuthenticationService.Instance.PlayerId);
        }
    }
    public async void CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);
            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log(joinCode);
            gameManager = GameObject.FindGameObjectWithTag("Game Manager");
            if (gameManager!= null)
            {
                gameManager.GetComponent<GameManager>().joinCode = joinCode;

            }

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
                );
            NetworkManager.Singleton.StartHost();
            GameManager gm = FindObjectOfType<GameManager>();
            gm.LoadSceneClientRpc("HubLevel");


        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }
    public async void JoinRelay()
    {
        try
        {
            
            string joinCode = iF.text;
            Debug.Log("Joining Relay with code " + joinCode);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData
                );
            NetworkManager.Singleton.StartClient();




        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

}
