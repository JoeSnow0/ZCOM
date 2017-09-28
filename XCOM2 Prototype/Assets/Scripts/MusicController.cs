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

    private void Awake()
    {
        
        DontDestroyOnLoad(this.gameObject);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    // Use this for initialization
    void Start () {

        

        SceneManager.GetSceneByBuildIndex(0);
    }
	
	// Update is called once per frame
	void Update () {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(0))
        {
            PlaySound(0, true);
            PlaySound(1, true);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            SceneManager.LoadScene(0);
        }
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(3))
        {
            PlaySound(2, true);
            PlaySound(3, true);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            PlaySound(2);
        }
        

    }

    public void PlaySound(int soundIndex, bool isLooping = false)
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

        audioSource = MundoSound.Play(soundClip[soundIndex], volumeMaster, Vector3.zero, isLooping, 0, transform);
        playingSources.Add(audioSource);

        //audioSource.loop = isLooping;

        //audioSource.clip = soundClip[soundIndex];
        //audioSource.Play();
    }

    void StopSound(int soundIndex)
    {

    }
}
