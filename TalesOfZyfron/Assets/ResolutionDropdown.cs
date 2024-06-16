using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static MenuManager;

public class ResolutionDropdown : MonoBehaviour
{
    public Dropdown resolutionDropdown;

    Resolution[] resolutions;
    // Start is called before the first frame update
    void Start()
    {
        resolutions = Screen.resolutions;
        
        List<string> resOptions = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            resOptions.Add(option);
        }
        resolutionDropdown.AddOptions(resOptions);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
