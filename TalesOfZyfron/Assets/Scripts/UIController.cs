using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Slider musicSlider,sfxSlider;
    public AudioManager audioManager;
    void Start()
    {
        if(audioManager is null)
        {
            audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        }
    }

    public void MusicVolume()
    {
        audioManager.MusicVolume(musicSlider.value);
    }
    public void SFXVolume()
    {
        audioManager.SFXVolume(sfxSlider.value);
    }
}
