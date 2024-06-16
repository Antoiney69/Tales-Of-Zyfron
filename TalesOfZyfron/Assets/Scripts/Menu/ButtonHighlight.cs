using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
   
    [SerializeField] TextMeshProUGUI text;
    public void ResetColor()
    {
        text.color = Color.white;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.color = Color.black; 
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.color = Color.white; 
    }

}
