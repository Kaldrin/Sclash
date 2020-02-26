using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayParticleSystem : MonoBehaviour
{
    [SerializeField] bool playParticleSystem = false;
    [SerializeField] bool stopParticleSytem = false;
    [SerializeField] ParticleSystem particleSystemToPlay = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (stopParticleSytem)
        {
            particleSystemToPlay.Stop();
        }

        if (playParticleSystem)
        {
            particleSystemToPlay.Play();
        }
    }
}
