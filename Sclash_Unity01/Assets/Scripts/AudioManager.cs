using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // SOUND FUNCTIONS
    [SerializeField] SoundFunctions soundFunctions = null;



    // FX
    // Audio sources
    [SerializeField]
    AudioSource
        walk = null,
        clash = null,
        parry = null,
        parryOn = null,
        lightAttack = null,
        heavyAttack = null,
        successfulAttack = null,
        death = null;

    /*
    [SerializeField] AudioSource
        charge = null,
        dash = null;
        */



    // MUSIC
    [SerializeField] AudioSource
        menuMusicSource = null,
        battleMusicSource = null;
    float menuMusicObjective = 1;
    float battleMusicObjective = 0;
    bool battleOn = false;
    // bool fadeMusic = false;
    bool menuMusicFinishedFade = false;
    bool battleMusicFinishedFade = false;
    [SerializeField] float musicFadeSpeed = 0.001f;
    float volumeComparisonTolerance = 0.1f;



    // VOICE
    [SerializeField] AudioSource deathVoice;
    [SerializeField] AudioSource attackVoice;
    [SerializeField] AudioSource introVoice;



    // PLAYERS
    GameObject[] players;
    float distanceBetweenPlayers = 0;
    [SerializeField] Vector2 battleMusicVolumeDistanceBounds = new Vector2(5, 20);
    [SerializeField] Vector2 battleMusicVolumeBounds = new Vector2(1, 0);



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
        


        FadeMusic();
    }

    public void FindPlayers()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
    }


    // MUSIC
    void ActivateMusicFade()
    {
        //fadeMusic = true;
        menuMusicFinishedFade = false;
        battleMusicFinishedFade = false;
    }
    
    void FadeMusic()
    {
        // Menu music
        if (menuMusicSource.volume > menuMusicObjective)
        {
            menuMusicSource.volume += -musicFadeSpeed;
        }
        else if (menuMusicSource.volume < menuMusicObjective)
        {
            menuMusicSource.volume += musicFadeSpeed;
        }
        else if (Mathf.Abs(menuMusicSource.volume - menuMusicObjective) < volumeComparisonTolerance)
        {
            menuMusicFinishedFade = true;
            menuMusicSource.volume = menuMusicObjective;
        }

        // Battle music
        if (battleMusicSource.volume > battleMusicObjective)
        {
            battleMusicSource.volume += -musicFadeSpeed;
        }
        else if (battleMusicSource.volume < battleMusicObjective)
        {
            battleMusicSource.volume += musicFadeSpeed;
        }
        else if (Mathf.Abs(battleMusicSource.volume - battleMusicObjective) < volumeComparisonTolerance)
        {
            battleMusicFinishedFade = true;
            battleMusicSource.volume = battleMusicObjective;
        }



        if (menuMusicFinishedFade && battleMusicFinishedFade)
        {
            // fadeMusic = false;
            battleMusicFinishedFade = false;
            menuMusicFinishedFade = false;

            battleMusicSource.volume = battleMusicObjective;
            menuMusicSource.volume = menuMusicObjective;
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
        distanceBetweenPlayers = Mathf.Abs(players[0].transform.position.x - players[1].transform.position.x);
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
        StartCoroutine(SuccessfulAttackCoroutine());
    }

    IEnumerator SuccessfulAttackCoroutine()
    {
        soundFunctions.PlaySoundFromSource(successfulAttack);

        yield return new WaitForSeconds(0f);
        soundFunctions.PlaySoundFromSource(death);
    }


    // Walk sound
    public void Walk(bool state)
    {
        soundFunctions.SetAudioActiveFromSource(walk, true);
        soundFunctions.SetAudioMuteFromSource(walk, !state);
    }
}
