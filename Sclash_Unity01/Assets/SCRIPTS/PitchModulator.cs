using UnityEngine;
using UnityEngine.Audio;





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






    private void Awake()                                                                // AWAKE
    {
        if (!sourceToModulate)
            sourceToModulate = GetComponent<AudioSource>();


        basePitch = sourceToModulate.pitch;
    }

    private void OnEnable()                                                         // ON ENABLE
    {
        sourceToModulate.pitch = Random.Range(basePitch * pitchModulationLimits.x, basePitch * pitchModulationLimits.y);
    }

    // Update is called once per frame
    void Update()                                                                                   // UPDATE
    {
        if (enabled && isActiveAndEnabled)
        {
            if (sourceToModulate != null && sourceToModulate.enabled && !modulateOnEnabled )
            {
                if (!sourceToModulate.isPlaying && oldIsPlaying)
                    sourceToModulate.pitch = Random.Range(basePitch * pitchModulationLimits.x, basePitch * pitchModulationLimits.y);


                oldIsPlaying = sourceToModulate.isPlaying;
            }
        }
    }
    



    public void Play()                                                                                          // PLAY
    {
        if (sourceToModulate)
        {
            sourceToModulate.pitch = Random.Range(basePitch * pitchModulationLimits.x, basePitch * pitchModulationLimits.y);
            sourceToModulate.Play();
        }
    }




    // EDITOR ONLY
    private void OnDrawGizmosSelected()
    {
        if (!sourceToModulate)
            sourceToModulate = GetComponent<AudioSource>();
    }
}
