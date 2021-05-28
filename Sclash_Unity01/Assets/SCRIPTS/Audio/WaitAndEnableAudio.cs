using System.Collections;
using System.Collections.Generic;
using UnityEngine;





// Bastien BERNAND
// Reusable asset

/// <summary>
/// Simple script that delays the volume set of an AudioSource once started. Yes I needed this very precise thing.
/// </summary>

// UNITY 2020.3.3
public class WaitAndEnableAudio : MonoBehaviour
{
    [SerializeField] AudioSource audioSource = null;
    [SerializeField] float volumeWaitTime = 2f;
    float audioSourceBaseVolume = 0f;





    void Start()                                                                                                             // START
    {
        Activated();
    }


    void Activated()                                                                                                             // ACTIVATED
    {
        GetMissingComponent();
        if (audioSource)
        {
            audioSourceBaseVolume = audioSource.volume;
            audioSource.volume = 0;
        }
        Invoke("SetVolume", volumeWaitTime);
    }


    void SetVolume()                                                                                                        // SET VOLUME
    {
        if (audioSource)
            audioSource.volume = audioSourceBaseVolume;
    }






    // Checks for missing components and gets them if it can
    void GetMissingComponent()                                                                                              // GET MISSING COMPONENT
    {
        if (audioSource == null && GetComponent<AudioSource>())
            audioSource = GetComponent<AudioSource>();
    }





    // When selected automatically gets the components
    private void OnDrawGizmosSelected()                                                                                             // ON DRAW GIZMOS SELECTED
    {
        GetMissingComponent();
    }
}
