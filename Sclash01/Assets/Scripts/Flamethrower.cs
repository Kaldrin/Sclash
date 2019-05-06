using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flamethrower : SoundManager {

    public ParticleSystem[]
        particlesToActivate;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Fire();
    }

    void Fire()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            for (int i = 0; i < particlesToActivate.Length; i++)
                particlesToActivate[i].Play();
            SetAudioMute("FireSound", false);
        } 
        else if (Input.GetButtonUp("Fire1"))
        {
            for (int i = 0; i < particlesToActivate.Length; i++)
                particlesToActivate[i].Stop();
            SetAudioMute("FireSound", true);
        }
    }
}
