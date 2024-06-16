using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
public class FindCodeRoom : NetworkBehaviour
{
    public TextMeshPro Text;
    void Start()
    {
        Text.text = "Le code de la partie est:\n" + GameObject.Find("GameManager").GetComponent<GameManager>().joinCode;
    }
}
