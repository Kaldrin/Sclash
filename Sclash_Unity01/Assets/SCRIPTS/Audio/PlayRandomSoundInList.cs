using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This script is to improve audio sources and allow to play a random sound in a list of sounds 
public class PlayRandomSoundInList : MonoBehaviour
{
    [SerializeField] bool play = false;
    [SerializeField] bool justPlayAudioSourceSound = false;
    [SerializeField] public AudioSource audioSource = null;
    [SerializeField] AudioClip[] soundList = null;
    [SerializeField] bool playOnEnable = false;






    #region FUNCTIONS
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


    private void OnEnable()                                                                 // ON ENABLE
    {
        if (playOnEnable)
            Play();
    }





    // PLAY
    public void Play()
    {
        play = true;
    }

    public void Stop()
    {
        play = false;
        audioSource.Stop();
    }





    // EDITOR ONLY
    private void OnDrawGizmos()
    {
        // Get audio source automatically
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }
    #endregion
}
