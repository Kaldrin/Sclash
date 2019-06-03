using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Sound functions
    [SerializeField] SoundFunctions soundFunctions;

    // FX
    [SerializeField] AudioSource walk;
    [SerializeField] AudioSource dash;
    [SerializeField] AudioSource clash;
    [SerializeField] AudioSource parry;
    [SerializeField] AudioSource parryOn;
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




    //FX
    public void Clash()
    {
        soundFunctions.PlaySoundFromSource(clash);
    }

    // Parry sound
    public void Parried()
    {
        soundFunctions.PlaySoundFromSource(parry);
    }

    public void ParryOn()
    {
        
        soundFunctions.PlaySoundFromSource(parryOn);
    }

    // Attack sound depending on level
    public void Attack(int level, int maxLevel)
    {
        if (level >= maxLevel)
        {
            soundFunctions.PlaySoundFromSource(heavyAttack);
        }
        else
        {
            soundFunctions.PlaySoundFromSource(lightAttack);
        } 
    }

    // Successful attack sound
    public void SuccessfulAttack()
    {
        soundFunctions.PlaySoundFromSource(successfulAttack);
    }


    // Walk sound
    public void Walk(bool state)
    {
        soundFunctions.SetAudioActiveFromSource(walk, true);
        soundFunctions.SetAudioMuteFromSource(walk, !state);
    }
}
