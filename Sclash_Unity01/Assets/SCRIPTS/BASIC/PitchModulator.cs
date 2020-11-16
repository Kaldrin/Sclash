using UnityEngine;

// Plugged on an audio source, this script allows to modulate the pitch of the source when it plays for variation purposes
// OPTIMIZED
[RequireComponent(typeof(AudioSource))]
public class PitchModulator : MonoBehaviour
{
    [SerializeField] AudioSource sourceToModulate = null;
    [SerializeField] Vector2 pitchModulationLimits = new Vector2(0.8f, 1.2f);
    [Tooltip("Should the pitch modulation happen when the script is enabled ? If not, will happen when the audio source starts playing")]
    [SerializeField] bool modulateOnEnabled = false;
    bool oldIsPlaying = false;
    float basePitch = 0;






    private void Awake()
    {
        if (!sourceToModulate)
        {
            sourceToModulate = new AudioSource();
            Debug.Log("The audio source to modulate the pitch was not referenced on this object : " + gameObject.name + ", ignoring");
        }
        else
            basePitch = sourceToModulate.pitch;
    }

    private void OnEnable()
    {
        sourceToModulate.pitch = Random.Range(pitchModulationLimits.x, pitchModulationLimits.y);
    }

    // Update is called once per frame
    void Update()
    {
        if (sourceToModulate != null && sourceToModulate.enabled && !modulateOnEnabled && enabled && isActiveAndEnabled)
        {
            oldIsPlaying = sourceToModulate.isPlaying;

            if (!sourceToModulate.isPlaying && oldIsPlaying)
                sourceToModulate.pitch = Random.Range(basePitch * pitchModulationLimits.x, basePitch * pitchModulationLimits.y);
        }
    }
}
