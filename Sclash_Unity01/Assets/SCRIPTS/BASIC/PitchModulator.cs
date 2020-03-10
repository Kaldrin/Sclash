using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitchModulator : MonoBehaviour
{
    [SerializeField] AudioSource sourceToModulate = null;
    bool oldIsPlaying = false;
    [SerializeField] Vector2 pitchModulationLimits = new Vector2(0.8f, 1.2f);
    [SerializeField] bool modulateOnEnabled = false;


    // Start is called before the first frame update
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
