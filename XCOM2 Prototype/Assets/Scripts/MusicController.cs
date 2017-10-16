using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using MundoSoundUtil;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


[RequireComponent(typeof(AudioSource))]


public class MusicController : MonoBehaviour {

    //initialization and tooltips

    static MusicController _instance = null;

    static List<AudioSource> playingSources = new List<AudioSource>();
    [SerializeField] float volumeMaster = 0.75f;
    [SerializeField] float volumeEffects = 0.75f; // <-- NOT IN USE
    [SerializeField] float volumeMusic = 0.75f; // <-- NOT IN USE

    [Header("A list of audioclips available to this specific audio source.")]
    [Tooltip("Each audioclip has a soundIndex equal to where it is in the list.")]
    public AudioClip[] soundClip;

    [Tooltip("Audio Source used:")]
    public AudioSource audioSource;
   
    bool isPlaying = false;
    int gameScreen;

    //Text volumeSliderValue;// <-- NOT IN USE

    private void Awake()
    {
        //removes audioSource duplicates
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
            
        
        //gets Scene Build Index
        gameScreen = SceneManager.GetActiveScene().buildIndex;

        //adds audioSource to itself
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    // Use this for initialization
    void Start () {


    }
	
	// Update is called once per frame
	void Update () {

        //Stops music between sceens
        if (gameScreen != SceneManager.GetActiveScene().buildIndex)
        {
            StopAllSound();
            gameScreen = SceneManager.GetActiveScene().buildIndex;

        }
        //Plays MainMenu music
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("mainMenu") && !isPlaying)
        {

            PlaySound(0, true);
            PlaySound(1, true);
            isPlaying = true;

        }
        //Plays GameSession music
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("gameSession") && !isPlaying)
        {
            PlaySound(2, true);
            PlaySound(3, true);
            isPlaying = true;
        }
    }

    //plays an AudioClip from the AudioSource
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

    //stops the sound with matching index
    public void StopSound(int soundIndex)
    {
        foreach (var audioSource in playingSources)
        {
            if (audioSource.clip.name == soundClip[soundIndex].name)
                audioSource.Stop();
        }
        isPlaying = false;
    }

    //stops all sound
    public void StopAllSound()
    {
        for (int i = 0; i < soundClip.Length; i++)
        {
            StopSound(i);

        }
    }

    public void AdjustVolume(float newVolume)
    {
        foreach (var audioSource in playingSources)
        {
            audioSource.volume = newVolume;
        }

        
    }
}