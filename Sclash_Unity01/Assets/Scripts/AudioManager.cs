using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Sound functions
    [SerializeField] SoundFunctions soundFunctions;



    // FX
    // Audio sources
    [SerializeField] AudioSource walk;
    [SerializeField] AudioSource dash;
    [SerializeField] AudioSource clash;
    [SerializeField] AudioSource parry;
    [SerializeField] AudioSource parryOn;
    [SerializeField] AudioSource lightAttack;
    [SerializeField] AudioSource heavyAttack;
    [SerializeField] AudioSource successfulAttack;
    [SerializeField] AudioSource charge;
    // Audio clip
    [SerializeField] AudioClip mainMenuMusic;
    [SerializeField] AudioClip battleMusic;



    // MUSIC
    [SerializeField] AudioSource menuMusicSource;
    float menuMusicObjective = 1;
    [SerializeField] AudioSource battleMusicSource;
    float battleMusicObjective = 0;
    bool battleOn = false;
    bool fadeMusic = false;
    bool menuMusicFinishedFade = false;
    bool battleMusicFinishedFade = false;



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
        if (battleOn)
        {
            AdjustMusicVolumeOnDistance();
        }
        

        if (fadeMusic)
        {
            FadeMusic();
        }
    }



    // MUSIC
    void ActivateMusicFade()
    {
        fadeMusic = true;
        menuMusicFinishedFade = false;
        battleMusicFinishedFade = false;
    }
    
    void FadeMusic()
    {
        // Menu music
        if (menuMusicSource.volume > menuMusicObjective)
        {
            menuMusicSource.volume += -0.01f;
        }
        else if (menuMusicSource.volume < menuMusicObjective)
        {
            menuMusicSource.volume += 0.01f;
        }
        else
        {
            menuMusicSource.volume = menuMusicObjective;
        }

        // Battle music
        if (battleMusicSource.volume > battleMusicObjective)
        {
            battleMusicSource.volume += -0.01f;
        }
        else if (battleMusicSource.volume < battleMusicObjective)
        {
            battleMusicSource.volume += 0.01f;
        }
        else
        {
            battleMusicSource.volume = battleMusicObjective;
        }
    }

    public void MenuMusicOn()
    {
        //soundFunctions.ChangeClipOfAudioSource(mainMusicSource, mainMenuMusic);
        soundFunctions.SetAudioActiveFromSource(menuMusicSource, true);
        //soundFunctions.SetAudioActiveFromSource(battleMusicSource, false);

        menuMusicObjective = 1;
        battleMusicObjective = 0;

        ActivateMusicFade();
    }  

    public void BattleMusicOn()
    {
        //soundFunctions.ChangeClipOfAudioSource(menuMusicSource, battleMusic);
        soundFunctions.SetAudioActiveFromSource(battleMusicSource, true);

        battleOn = true;
        menuMusicObjective = 0;
        battleMusicObjective = 1;

        ActivateMusicFade();
    }

    void AdjustMusicVolumeOnDistance()
    {

    }



    // FX
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
