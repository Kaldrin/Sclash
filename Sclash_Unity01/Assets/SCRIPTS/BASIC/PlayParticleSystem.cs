using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Simple script used to play a particle system from an animation
// OPTIMIZED
[RequireComponent(typeof(ParticleSystem))]
public class PlayParticleSystem : MonoBehaviour
{
    [SerializeField] bool playParticleSystem = false;
    [SerializeField] bool stopParticleSytem = false;
    [SerializeField] ParticleSystem particleSystemToPlay = null;







    void Start()
    {
        if (particleSystemToPlay == null)
            particleSystemToPlay = GetComponent<ParticleSystem>();
    }


    void Update()
    {
        if (enabled && isActiveAndEnabled)
        {
            // Stop
            if (stopParticleSytem && particleSystemToPlay.isPlaying)
                particleSystemToPlay.Stop();

            // Play
            if (playParticleSystem && !particleSystemToPlay.isPlaying) 
                particleSystemToPlay.Play();
        }
    }






    // EDITOR ONLY
    private void OnDrawGizmos()
    {
        if (particleSystemToPlay == null)
            particleSystemToPlay = GetComponent<ParticleSystem>();
    }
}
