using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayRandomSoundInList : MonoBehaviour
{
    [SerializeField] bool play = false;
    [SerializeField] bool justPlayAudioSourceSound = false;
    [SerializeField] AudioSource audioSource = null;
    [SerializeField] AudioClip[] soundList = null;




    // BASE FUNCTIONS
    // FixedUpdate is called 30 times per second
    void FixedUpdate()
    {
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
}
