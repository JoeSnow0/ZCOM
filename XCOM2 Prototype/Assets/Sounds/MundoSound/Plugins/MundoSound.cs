//------------------------------------------------------------------------------------------------------
// MundoSound.cs is a class with static helper methods to help usign sound in Unity.
// You can use this class freely in any project that you want without credits needed.
// Author: Luis Guilherme P.C - email: itsluisg@gmail.com 
// By: MundoSound
// If you need affordable high quality royality free audio, don't forget to visit www.mundosound.com
//-------------------------------------------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MundoSoundUtil;

public static class MundoSound {
	
	private static Transform audioPoolContainer;
	private static List<AudioSource> audioPoolList = new List<AudioSource>();

	private static GameObject Ins(string name){		
		return new GameObject(name);		
	}

	/// <summary>
	/// Play sound with a specific AudioClip, Volume, Position, if it will loop, delay and a container to be placed this audioSource gameobject's.
	///	This method will recycle AudioSource object.
	/// </summary>
	/// <returns>The sound.</returns>
	/// <param name="clip">Clip.</param>
	/// <param name="volume">Volume.</param>
	/// <param name="pos">Position.</param>
	/// <param name="loop">If set to <c>true</c> loop.</param>
	/// <param name="delay">Delay.</param>
	public static AudioSource Play(AudioClip clip, float volume, Vector3 offsetPosition, bool loop, float delay, Transform container){
		
		//Searches for an audio container
		if(audioPoolContainer == null){
			audioPoolContainer = Ins ("MundoSound_Audio_Pool").transform;
		}			

		//Loop in the list
		for (int i = 0; i < audioPoolList.Count; i++) {
			
			//Safety check if the AudioSource object has been deleted
			if(audioPoolList[i] == null){
				audioPoolList.RemoveAt(i);
			}
			//If this audio in not null
			else{
				//And if this audio is no playing
				if(!audioPoolList[i].isPlaying){

					//Reset a few parameters
					audioPoolList[i].name = "Sound: " + clip.name;
					audioPoolList[i].clip = clip;
					audioPoolList[i].volume = volume;
					audioPoolList[i].pitch = 1;
					audioPoolList[i].loop = loop;
					if(container == null){
						audioPoolList[i].transform.parent = audioPoolContainer;
					}
					else{
						audioPoolList[i].transform.parent = container;
					}
					audioPoolList[i].transform.localPosition = offsetPosition;

					if(delay == 0) audioPoolList[i].Play();					
					else audioPoolList[i].PlayDelayed (delay);				

					//Then let's return this audioSource
					return audioPoolList[i];	
				}				
			}
		}

		//If it dosen't find any available audio, then let's create a new object
		GameObject go = Ins ("Sound: " + clip.name);

		//And add an AudioSource to it
		AudioSource newAudioSource = go.AddComponent<AudioSource> ();

		//Let's configure this audios souce with the information we got in the calling method
		newAudioSource.clip = clip;
		newAudioSource.loop = loop;
		newAudioSource.volume = volume;
		newAudioSource.playOnAwake = false;

		//Should we play this audio delayed?
		if(delay == 0) newAudioSource.Play();
		//If so, do it
		else newAudioSource.PlayDelayed(delay);

		//Check if there's a specific contatiner to put this audioSoruce gameobject
		if(container == null){
			//Let's attach this new gameObject containing the audio source to a container
			//This way we can keep the hierarchy clean
			go.transform.parent = audioPoolContainer;
		}
		else{
			go.transform.parent = container;
		}

		go.transform.localPosition = offsetPosition;

		//Add to our audiosource list, so we can recycle it later
		audioPoolList.Add (newAudioSource);
		
		//Return this audioSource
		return newAudioSource;
	}
	
	/// <summary>
	/// Play sound with a specific AudioClip, Volume, Position and if it will loop.
	/// This method will recycle AudioSource object.
	/// </summary>
	/// <param name="clip">Clip.</param>
	/// <param name="volume">Volume.</param>
	/// <param name="pos">Position.</param>
	/// <param name="loop">If set to <c>true</c> loop.</param>
	/// <param name="delay">Delay.</param>
	public static AudioSource Play(AudioClip clip, float volume, Vector3 pos, bool loop){
		return Play (clip, volume, pos, loop, 0, null);		
	}
	
	/// <summary>
	/// Play sound with a specific AudioClip, Volume and if it will loop.
	/// This method will recycle AudioSource object.
	/// </summary>
	/// <param name="clip">Clip.</param>
	/// <param name="volume">Volume.</param>
	/// <param name="loop">If set to <c>true</c> loop.</param>
	public static AudioSource Play(AudioClip clip, float volume, bool loop){
		return Play (clip, volume, Vector3.zero, loop, 0, null);		
	}
	
	/// <summary>
	/// Play sound with a specific AudioClip, Volume and Position.
	/// This method will recycle AudioSource object.
	/// </summary>
	/// <param name="clip">Clip.</param>
	/// <param name="volume">Volume.</param>
	/// <param name="pos">Position.</param>
	public static AudioSource Play(AudioClip clip, float volume, Vector3 pos){
		return Play (clip, volume, pos, false, 0, null);		
	}
	
	/// <summary>
	/// Play sound with a specific AudioClip and Volume and if it will loop.
	/// This method will recycle AudioSource object.
	/// </summary>
	/// <param name="clip">Clip.</param>
	/// <param name="loop">If set to <c>true</c> loop.</param>
	public static AudioSource Play(AudioClip clip, bool loop){
		return Play (clip, 1, Vector3.zero, loop, 0, null);		
	}

	
	/// <summary>
	/// Play sound with a specific AudioClip and Volume.
	/// This method will recycle AudioSource object.
	/// </summary>
	/// <param name="clip">Clip.</param>
	/// <param name="volume">Volume.</param>
	public static AudioSource Play(AudioClip clip, float volume){
		return Play (clip, volume, Vector3.zero, false, 0, null);		
	}
	
	/// <summary>
	/// Play sound with a specific AudioClip.
	/// This method will recycle AudioSource object.
	/// </summary>
	/// <param name="clip">Clip.</param>
	public static AudioSource Play(AudioClip clip){
		return Play (clip, 1, Vector3.zero, false, 0, null);		
	}
		
	/// <summary>
	/// Play sound with a specific AudioClip, volume and container.
	/// This method will recycle AudioSource object.
	/// </summary>
	/// <param name="clip">Clip.</param>
	public static AudioSource Play(AudioClip clip, float volume, Transform container){
		return Play (clip, 1, Vector3.zero, false, 0, container);		
	}

	/// <summary>
	/// Play sound with a specific AudioClip and container.
	/// This method will recycle AudioSource object.
	/// </summary>
	/// <param name="clip">Clip.</param>
	public static AudioSource Play(AudioClip clip, Transform container){	
		return Play (clip, 1, Vector3.zero, false, 0, container);		
	}

	/// <summary>
	/// Play sound with a specific AudioClip, position offset and container.
	/// This method will recycle AudioSource object.
	/// </summary>
	/// <param name="clip">Clip.</param>
	public static AudioSource Play(AudioClip clip, Vector3 offsetPosition, Transform container){
		return Play (clip, 1, offsetPosition, false, 0, container);
	}


	//----------- FADING ------------//

	/// <summary>
	/// Fades the volume in linearly.
	/// If the sound is not playing, you must call audioSource.Play() first.
	/// </summary>
	/// <param name="audioSource">Audio source.</param>
	/// <param name="time">Time.</param>
	public static void FadeVolumeIn(AudioSource audioSource, float time){
		FadeVolume(audioSource, time, 1);
	}
		
	/// <summary>
	/// Fades the volume out linearly.
	/// If the sound is not playing, you must call audioSource.Play() first.
	/// </summary>
	/// <param name="audioSource">Audio source.</param>
	/// <param name="time">Time.</param>
	public static void FadeVolumeOut(AudioSource audioSource, float time){
		FadeVolume(audioSource, time, 0);
	}

	/// <summary>
	/// Fades the volume linearly to a target Volume.
	/// If the sound is not playing, you must call audioSource.Play() first.
	/// </summary>
	/// <param name="audioSource">Audio source.</param>
	/// <param name="toVolume">To volume.</param>
	/// <param name="time">Time.</param>
	public static void FadeVolume(AudioSource audioSource, float time, float targetVolume){
		MSAudioVolume mSAudioVolume = audioSource.GetComponent<MSAudioVolume>();
		if(mSAudioVolume == null){
			mSAudioVolume = audioSource.gameObject.AddComponent<MSAudioVolume>();
		}
		mSAudioVolume.Fade(targetVolume, time);
	}

	/// <summary>
	/// Fades the volume using animation curve.
	/// If the sound is not playing, you must call audioSource.Play() first.
	/// </summary>
	/// <param name="audioSource">Audio source.</param>
	/// <param name="toVolume">To volume.</param>
	/// <param name="time">Time.</param>
	public static void FadeVolume(AudioSource audioSource, float time,  AnimationCurve interpolation){
		MSAudioVolume mSAudioVolume = audioSource.GetComponent<MSAudioVolume>();
		if(mSAudioVolume == null){
			mSAudioVolume = audioSource.gameObject.AddComponent<MSAudioVolume>();
		}
		mSAudioVolume.Fade(interpolation, time);
	}
}
