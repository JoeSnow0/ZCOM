using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using MundoSoundUtil;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]

public class MusicController : MonoBehaviour {

    static List<AudioSource> playingSources = new List<AudioSource>();

    float volumeMaster = 0.75f;
    float volumeEffects = 0.75f;
    float volumeMusic = 0.75f;

    [Header("A list of audioclips available to this specific audio source.")]
    [Tooltip("Each audioclip has a soundIndex equal to where it is in the list.")]
    public AudioClip[] soundClip;

    [Tooltip("Audio Source used:")]
    public AudioSource audioSource;
    bool isPlaying = false;

    private void Awake()
    {
        
        DontDestroyOnLoad(this.gameObject);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        
    }

    // Use this for initialization
    void Start () {

        

        //SceneManager.GetSceneByBuildIndex(0);
    }
	
	// Update is called once per frame
	void Update () {

        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(0) && !isPlaying)
        {
            PlaySound(0, true);
            PlaySound(1, true);
            isPlaying = true;

        }
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(3) && !isPlaying)
        {
            PlaySound(2, true);
            PlaySound(3, true);
            isPlaying = true;
        }

        //debug load main menu scene
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    SceneManager.LoadScene(0);
            
        //}
        //if (Input.GetKeyDown(KeyCode.L))
        //{


        //    StopAllSound();
            

        //}

    }

    public void PlaySound(int soundIndex, bool isLooping = false)
    {              
        foreach( var audioSource in playingSources)
        {
            
            if (!audioSource.isPlaying)
                break;
            if (audioSource.clip.name == soundClip[soundIndex].name)
                return;
        }
        audioSource = MundoSound.Play(soundClip[soundIndex], volumeMaster, Vector3.zero, isLooping, 0, transform);
        playingSources.Add(audioSource);     
    }

    public void StopSound(int soundIndex)
    {
        foreach (var audioSource in playingSources)
        {
            if (audioSource.clip.name == soundClip[soundIndex].name)
                audioSource.Stop();
        }
        isPlaying = false;
    }

    public void StopAllSound()
    {
        for (int i = 0; i < soundClip.Length; i++)
        {
            StopSound(i);

        }
    }
}
