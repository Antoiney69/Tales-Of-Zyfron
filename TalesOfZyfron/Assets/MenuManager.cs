using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject menu_1;
    [SerializeField] GameObject menu_2;
    [SerializeField] GameObject playButton;
    [SerializeField] GameObject playButtonSelected;
    [SerializeField] GameObject playMenu;
    [SerializeField] GameObject settingsButton;
    [SerializeField] GameObject settingsButtonSelected;
    [SerializeField] GameObject settingsMenu;
    ButtonHighlight playButtonHighlight;
    ButtonHighlight settingsButtonHighlight; 
    public MenuState currMenuState;


    public TMP_Dropdown resoutionDropdown;
    Resolution[] resolutions;
    public enum MenuState
    {
        menu_1, menu_2,


    }
    
    
    private void Start()
    {
        Screen.fullScreen = true;
        resolutions = Screen.resolutions;
        currMenuState = MenuState.menu_1;
        List<string> resOptions = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height; 
            resOptions.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        resoutionDropdown.AddOptions(resOptions);
        resoutionDropdown.value = currentResolutionIndex;
        resoutionDropdown.RefreshShownValue();

        menu_2.SetActive(false);
        playMenu.SetActive(false);
        settingsMenu.SetActive(false);

        playButtonSelected.SetActive(false);
        settingsButtonSelected.SetActive(false);
        playButtonHighlight = playButton.GetComponent<ButtonHighlight>();
        settingsButtonHighlight = settingsButton.GetComponent<ButtonHighlight>();

    }


    // Update is called once per frame
    void Update()
    {
        

        if (Input.GetKeyDown(KeyCode.Space) && currMenuState == MenuState.menu_1) 
        {
            menu_1.SetActive(false);
            menu_2.SetActive(true);
            currMenuState = MenuState.menu_2;
           
        }
        
    }

    public void OnPlayButtonClicked()
    {
        playButtonSelected.SetActive(true);
        playButton.SetActive(false);
        playMenu.SetActive(true);
        settingsButton.SetActive(true);
        settingsButtonSelected.SetActive(false);
        settingsButtonHighlight.ResetColor();
        settingsMenu.SetActive(false);

    }
    public void OnSettingsButtonClicked()
    {
        settingsButtonSelected.SetActive(true);
        settingsButton.SetActive(false);
        playMenu.SetActive(false);
        settingsMenu.SetActive(true);
        playButton.SetActive(true );
        playButtonSelected.SetActive(false);
        playButtonHighlight.ResetColor();
    }

    public void SetFullscreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
    }
    public void SetQuality(int quality)
    {
        QualitySettings.SetQualityLevel(quality);
    }
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

 


}
