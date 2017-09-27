//------------------------------------------------------------------------------------------------------
// You can use this class freely in any project that you want without credits needed.
// Author: Luis Guilherme P.C - email: itsluisg@gmail.com 
// By: MundoSound
// If you need affordable high quality royality free audio, don't forget to visit www.mundosound.com
//-------------------------------------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

namespace MundoSoundUtil{
	public class MSAudioVolume : MonoBehaviour {

		private AudioSource audioSource;

		private AnimationCurve animationCurve;
		private bool isUsingAnimationCurve;
		private bool isFading;
		private float targetVolume;
		private float time;
		private float currentTime;

		private void Awake(){
			audioSource = GetComponent<AudioSource>();
		}

		public void Fade(float newVolume, float time){
			isFading = true;
			this.targetVolume = newVolume;
			this.time = time;
			currentTime = 0;
		}

		public void Fade(AnimationCurve interpolation, float time){
			isFading = true;
			this.time = time;
			this.animationCurve = interpolation;
			isUsingAnimationCurve = true;
			currentTime = 0;
		}

		void Update(){
			
			if(!isFading) return;

			float volume = 0;

			//If is not using animation curve
			if(!isUsingAnimationCurve){
				volume = Mathf.Lerp(audioSource.volume, targetVolume, Time.deltaTime / (time - currentTime));
				currentTime += Time.deltaTime;

				if(currentTime >= time){
					audioSource.volume  = targetVolume;
					isFading = false;
				}	
			}
			//If is using animation curve
			else{
				currentTime += Time.deltaTime;
				volume = animationCurve.Evaluate(currentTime/time);

				//Check if audiosource time has ended
				if(currentTime >= time){
					isFading = false;
					isUsingAnimationCurve = false;
				}	
			}

			//Apply volume changes to audiosource
			audioSource.volume = volume;		
		}
	}
}