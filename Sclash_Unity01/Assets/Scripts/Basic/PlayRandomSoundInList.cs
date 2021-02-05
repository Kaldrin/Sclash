using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This script is to improve audio sources and allow to play a random sound in a list of sounds 
[RequireComponent(typeof(AudioSource))]
public class PlayRandomSoundInList : MonoBehaviour
{
    [SerializeField] bool play = false;
    [SerializeField] bool justPlayAudioSourceSound = false;
    [SerializeField] AudioSource audioSource = null;
    [SerializeField] AudioClip[] soundList = null;






    // BASE FUNCTIONS
    private void Start()                                                        // START
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }


    void FixedUpdate()                                                                      // FIXED UPDATE
    {
        if (enabled && isActiveAndEnabled)
            if (play)
            {
                if (justPlayAudioSourceSound)
                {
                    play = false;
                    audioSource.Play();
                }
                else
                {
                    play = false;
                    int randomSoundIndex = Random.Range(0, soundList.Length - 1);

                    audioSource.clip = soundList[randomSoundIndex];
                    audioSource.Play();
                }
            }
    }






    // PLAY
    public void Play()
    {
        play = true;
    }





    // EDITOR ONLY
    private void OnDrawGizmos()
    {
        // Get audio source automatically
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }
}
