using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFunctions : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    // Play sound from name of the game object with the audio source
    public void PlaySoundFromName(string sound)
    {
        AudioSource soundObject = null;

        if (GameObject.Find(sound).GetComponent<AudioSource>())
        {
            soundObject = GameObject.Find(sound).GetComponent<AudioSource>();
            soundObject.Play();
            soundObject.loop = false;
        }
        else
            Debug.Log("AudioSource '" + sound + "' could'nt be found in the scene");
    }

    // Play sound from audio source
    public void PlaySoundFromSource(AudioSource audioSource)
    {
        AudioSource soundObject = null;

        if (audioSource)
        {
            soundObject = audioSource;
            soundObject.Play();
            soundObject.loop = false;
        }
        else
            Debug.Log("AudioSource '" + audioSource + "' could'nt be found in the scene");
    }


    // Activate / deactivate an audiosource (Music, ambiance, etc) from name of the game object with the audio source
    public void SetAudioActiveFromName(string sound, bool state)
    {
        AudioSource soundObject = null;

        if (GameObject.Find(sound).GetComponent<AudioSource>())
        {
            soundObject = GameObject.Find(sound).GetComponent<AudioSource>();


            if (state)
                soundObject.Play();
            else
                soundObject.Stop();
            soundObject.loop = state;
        }
        else
            Debug.Log("AudioSource '" + sound + "' could'nt be found in the scene");
    }


    // Activate / deactivate an audiosource (Music, ambiance, etc)
    public void SetAudioActiveFromSource(AudioSource audioSource, bool state)
    {
        AudioSource soundObject = null;

        if (audioSource)
        {
            soundObject = audioSource;


            if (state)
                soundObject.Play();
            else
                soundObject.Stop();
            soundObject.loop = state;
        }
        else
            Debug.Log("AudioSource '" + audioSource + "' could'nt be found in the scene");
    }


    // Mute / unmute an audiosource (Music, ambiance, etc) from name of the game object with the audiosource
    public void SetAudioMuteFromName(string sound, bool state)
    {
        AudioSource soundObject = null;

        if (GameObject.Find(sound).GetComponent<AudioSource>())
        {
            soundObject = GameObject.Find(sound).GetComponent<AudioSource>();

            soundObject.mute = state;
        }
        else
            Debug.Log("AudioSource '" + sound + "' could'nt be found in the scene");
    }


    // Mute / unmute an audiosource (Music, ambiance, etc)
    public void SetAudioMuteFromSource(AudioSource audioSource, bool state)
    {
        AudioSource soundObject = null;

        if (audioSource)
        {
            soundObject = audioSource;

            soundObject.mute = state;
        }
        else
            Debug.Log("AudioSource '" + audioSource + "' could'nt be found in the scene");
    }


    // Change the audio source's clip
    public void ChangeClipOfAudioSource(AudioSource audioSource, AudioClip audioClip)
    {
        audioSource.clip = audioClip;
    }
}
