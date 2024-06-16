using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public float SFXVol  = 0.2f;
    public static AudioManager Instance { get { return _instance; } }

    [Header("Audio Source")]
    public  AudioSource musicSource;

    [Header("Audio Clip")]
    public AudioClip mainMenuTheme;
    [SerializeField] AudioClip[] MUSIC;
    [SerializeField] AudioClip[] SFX;

    void Awake()
    {
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
    }
    private void Start()
    {
        musicSource.clip = mainMenuTheme;
        musicSource.Play();
    }

    public void StartMusic()
    {
        musicSource.Stop();
        musicSource.loop = true;
        musicSource.clip = MUSIC[Random.Range(0,MUSIC.Length-1)];
        musicSource.Play();
    }

    public void MusicVolume(float val)
    {
        musicSource.volume = val;
    }

    public void SFXVolume(float val)
    {
        SFXVol = val;
    }


}
