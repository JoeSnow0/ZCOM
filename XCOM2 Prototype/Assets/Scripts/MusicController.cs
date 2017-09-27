using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
[RequireComponent(typeof(AudioSource))]

public class MusicController : MonoBehaviour {

    float volumeMaster = 0.75f;
    int volumeEffects = 100;
    int volumeMusic = 100;

    public AudioClip[] soundClip;
    public AudioSource audioSource;



	// Use this for initialization
	void Start () {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.A))
        {
            PlaySound(0);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            PlaySound(1);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            PlaySound(2);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            PlaySound(3, true);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            PlaySound(4);
        }
    }
    void PlaySound(int soundIndex, bool isLooping = false, bool playAlone = false)
    {
        //AudioSource aS = audioSource.GetComponent<AudioSource>();
        audioSource.Stop();
        audioSource.loop = isLooping;

        audioSource.clip = soundClip[soundIndex];
        audioSource.Play();
    }

    void MusicLoop(int SoundIndex)
    {

    }
}
