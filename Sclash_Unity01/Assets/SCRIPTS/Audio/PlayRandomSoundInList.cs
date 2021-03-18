using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// HEADER
// Reusable script


/// <summary>
/// This script is to improve audio sources and allow to play a random sound in a list of sounds
/// </summary>

// VERSION
// Mae for Unity 2019.1.1f1
public class PlayRandomSoundInList : MonoBehaviour
{
    [SerializeField] bool play = false;
    [SerializeField] bool justPlayAudioSourceSound = false;
    [SerializeField] public AudioSource audioSource = null;
    [SerializeField] public AudioClip[] soundList = null;
    [SerializeField] bool playOnEnable = false;
    [SerializeField] bool loopAudioSourceWithRanom = false;






    #region FUNCTIONS
    // BASE FUNCTIONS
    private void Start()                                                                                                                             // START
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }


    void FixedUpdate()                                                                                                                              // FIXED UPDATE
    {
        if (enabled && isActiveAndEnabled)
        {
            if (play)
                PlayRandom();

            if (audioSource != null && loopAudioSourceWithRanom && !audioSource.isPlaying)
                PlayRandom();
        }
    }


    private void OnEnable()                                                                                                                              // ON ENABLE
    {
        if (playOnEnable)
            Play();
    }





    // PLAY
    void PlayRandom()
    {
        play = false;

        if (justPlayAudioSourceSound)
        {
            if (audioSource != null)
                audioSource.Play();
        }
        else
        {
            int randomSoundIndex = 0;
            if (soundList != null)
                randomSoundIndex = Random.Range(0, soundList.Length - 1);

            if (audioSource != null && soundList != null && soundList.Length > randomSoundIndex)
            {
                audioSource.clip = soundList[randomSoundIndex];
                audioSource.Play();
            }
        }
    }
    
    
    public void Play()                                                                                                                                          // PLAY
    {
        play = true;
    }

    public void Stop()                                                                                                                                           // STOP
    {
        play = false;
        audioSource.Stop();
    }





    // EDITOR ONLY
    private void OnDrawGizmos()                                                                                                                                     // ON DRAW GIZMOS
    {
        // Get audio source automatically
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }
    #endregion
}
