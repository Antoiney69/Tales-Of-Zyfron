using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenLight : MonoBehaviour
{
    public GameObject Light;
    public ParticleSystem Particle;
    public GameObject Cache;
    public AudioSource source;
    public AudioClip clip;
    private bool isActive = true;
    void Start()
    {
        MarchePas();
    }
    private void MarchePas()
    {
        if(isActive)
        {
            Light.SetActive(true);
            Cache.SetActive(false);
        }
        else
        {
            Light.SetActive(false);
            Cache.SetActive(true);
            source.PlayOneShot(clip);
            Particle.Play();
        }
        isActive = !isActive;
        Invoke(nameof(MarchePas),Random.Range(0.25f,3.0f));
    }
}
