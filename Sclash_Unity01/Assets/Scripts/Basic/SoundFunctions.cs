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


    // Play sound from name of the game object with the audiosource
    public void PlaySound(string sound)
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


    // Activate an audiosource (Music, ambiance, etc) from name of the game object with the audiosource
    public void SetAudioActive(string sound, bool state)
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


    // Deactivate an audiosource (Music, ambiance, etc) from name of the game object with the audiosource
    public void SetAudioMute(string sound, bool state)
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
}
