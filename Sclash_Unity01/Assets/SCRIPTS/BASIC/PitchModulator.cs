using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitchModulator : MonoBehaviour
{
    [SerializeField] AudioSource sourceToModulate = null;
    bool oldIsPlaying = false;
    [SerializeField] Vector2 pitchModulationLimits = new Vector2(0.8f, 1.2f);
    [SerializeField] bool modulateOnEnabled = false;


    private void Awake()
    {
        if (!sourceToModulate)
        {
            sourceToModulate = new AudioSource();
            Debug.Log("The audio source to modulate the pitch was not referenced on this object : " + gameObject.name + ", ignoring");
        }
    }



    private void OnEnable()
    {
        sourceToModulate.pitch = Random.Range(pitchModulationLimits.x, pitchModulationLimits.y);
    }

    // Update is called once per frame
    void Update()
    {
        if (!modulateOnEnabled)
        {
            oldIsPlaying = sourceToModulate.isPlaying;

            if (!sourceToModulate.isPlaying && oldIsPlaying)
            {
                sourceToModulate.pitch = Random.Range(pitchModulationLimits.x, pitchModulationLimits.y);
            }
        }
    }
}
