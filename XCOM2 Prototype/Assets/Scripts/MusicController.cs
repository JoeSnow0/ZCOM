using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
[RequireComponent(typeof(AudioSource))]

public class MusicController : MonoBehaviour {

    float volumeMaster = 0.50f;
    int volumeEffects = 100;
    int volumeMusic = 100;

    public AudioClip[] soundClip;
    public AudioSource audioSource;



	// Use this for initialization
	void Start () {
        
        
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.A))
        {
            PlaySound(1);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            PlaySound(2);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            PlaySound(0);
        }
    }
    void PlaySound(int soundIndex)
    {
        audioSource.PlayOneShot(soundClip[soundIndex], volumeMaster);
        
    }
}
