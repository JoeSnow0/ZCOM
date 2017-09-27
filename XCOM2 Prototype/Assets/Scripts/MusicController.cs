using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using MundoSoundUtil;
[RequireComponent(typeof(AudioSource))]

public class MusicController : MonoBehaviour {

    static List<AudioSource> playingSources = new List<AudioSource>();

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
            PlaySound(0, true);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            PlaySound(1, true);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            PlaySound(2);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            PlaySound(3);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            PlaySound(4);
        }
        
    }
    void PlaySound(int soundIndex, bool isLooping = false)
    {
        //AudioSource aS = audioSource.GetComponent<AudioSource>();
        //audioSource.Stop();
        foreach( var audioSource in playingSources)
        {
            //playingSources.Remove(audioSource);
            if (!audioSource.isPlaying)
                break;
            if (audioSource.clip.name == soundClip[soundIndex].name)
                return;
        }

        audioSource = MundoSound.Play(soundClip[soundIndex], 0.5f, isLooping);
        playingSources.Add(audioSource);

        //audioSource.loop = isLooping;

        //audioSource.clip = soundClip[soundIndex];
        //audioSource.Play();
    }

    void StopSound(int soundIndex)
    {

    }
}
