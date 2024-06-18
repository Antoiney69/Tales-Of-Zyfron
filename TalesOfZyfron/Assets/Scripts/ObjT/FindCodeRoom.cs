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
        Text.text = "The Rooms Code:\n" + GameObject.Find("GameManager").GetComponent<GameManager>().joinCode;
    }
}
