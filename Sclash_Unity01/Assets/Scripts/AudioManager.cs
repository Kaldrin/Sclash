using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{ 

    // FX
    [SerializeField] AudioSource walk;
    [SerializeField] AudioSource dash;
    [SerializeField] AudioSource clash;
    [SerializeField] AudioSource parry;
    [SerializeField] AudioSource lightAttack;
    [SerializeField] AudioSource heavyAttack;
    [SerializeField] AudioSource successfulAttack;
    [SerializeField] AudioSource charge;

    // MUSIC
    [SerializeField] AudioSource mainMusicSource;

    // VOICE
    [SerializeField] AudioSource deathVoice;
    [SerializeField] AudioSource attackVoice;
    [SerializeField] AudioSource introVoice;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
