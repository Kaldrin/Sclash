using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // SOUND FUNCTIONS
    [Header("SOUND FUNCTIONS")]
    [SerializeField] SoundFunctions soundFunctions = null;





    // FX
    [Header("FX")]
    [SerializeField] AudioSource clash = null;
    [SerializeField] AudioSource
        parry = null,
        successfulAttack = null,
        death = null;
    [SerializeField] public PlayRandomSoundInList
        matchBegins = null,
        roundBegins = null;






    // MUSIC
    [Header("MUSIC")]
    [SerializeField] AudioSource menuMusicSource = null;
    [SerializeField] AudioSource
        battleMusicSource = null,
        windSource = null;
    [SerializeField] float
        menuMusicMaxVolume = 0.7f,
        battleMusicMaxVolume = 1f,
        windMaxVolume = 1f;
    float
        menuMusicObjective = 0.7f,
        battleMusicObjective = 0,
        windObjective = 0;
    bool
        battleOn = false,
        menuMusicFinishedFade = false,
        battleMusicFinishedFade = false,
        windFinishedFade = false;
    [SerializeField] float musicFadeSpeed = 0.001f;
    float volumeComparisonTolerance = 0.1f;






    // VOICE
    [Header("VOICE")]
    [SerializeField] AudioSource deathVoice;
    [SerializeField] AudioSource introVoice;






    // PLAYERS
    [Header("PLAYERS")]
    [SerializeField] Vector2 battleMusicVolumeDistanceBounds = new Vector2(5, 20);
    [SerializeField] Vector2 battleMusicVolumeBounds = new Vector2(1, 0);

    GameObject[] players;
    //float distanceBetweenPlayers = 0;











    // BASE FUNCTIONS
    // Start is called before the first frame update
    void Start()
    {
        menuMusicObjective = menuMusicMaxVolume;
        FindPlayers();
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
    void FadeMusic()
    {
        // Menu music
        if (menuMusicSource.volume > menuMusicObjective)
        {
            menuMusicSource.volume += -musicFadeSpeed;


            if (menuMusicSource.volume <= menuMusicObjective)
            {
                menuMusicSource.volume = menuMusicObjective;
                menuMusicFinishedFade = true;
                menuMusicSource.volume = menuMusicObjective;
            }
        }
        else if (menuMusicSource.volume < menuMusicObjective)
        {
            menuMusicSource.volume += musicFadeSpeed;

            if (menuMusicSource.volume >= menuMusicObjective)
            {
                menuMusicSource.volume = menuMusicObjective;
                menuMusicFinishedFade = true;
                menuMusicSource.volume = menuMusicObjective;
            }
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


        // Wind
        if (windSource.volume > windObjective)
        {
            windSource.volume += -musicFadeSpeed;
        }
        else if (windSource.volume < windObjective)
        {
            windSource.volume += musicFadeSpeed;
        }
        else if (Mathf.Abs(windSource.volume - windObjective) < volumeComparisonTolerance)
        {
            windFinishedFade = true;
            windSource.volume = battleMusicObjective;
        }


        if (menuMusicFinishedFade && battleMusicFinishedFade && windFinishedFade)
        {
            battleMusicFinishedFade = false;
            menuMusicFinishedFade = false;
            windFinishedFade = false;

            battleMusicSource.volume = battleMusicObjective;
            menuMusicSource.volume = menuMusicObjective;
            windSource.volume = windObjective;
        }
    }

    // Activate menu music
    public void MenuMusicOn()
    {
        soundFunctions.SetAudioActiveFromSource(menuMusicSource, true);

        battleOn = false;
        menuMusicObjective = menuMusicMaxVolume;
        battleMusicObjective = 0;
        windObjective = 0;

        //ActivateMusicFade();
    }  

    // Activate battle music
    public void BattleMusicOn()
    {
        soundFunctions.SetAudioActiveFromSource(battleMusicSource, true);

        battleOn = true;
        menuMusicObjective = 0;
        battleMusicObjective = battleMusicMaxVolume;
        windObjective = 0;

        //ActivateMusicFade();
    }

    // Activate wind
    public void WindOn()
    {
        soundFunctions.SetAudioActiveFromSource(windSource, true);

        battleOn = false;
        menuMusicObjective = 0;
        battleMusicObjective = 0;
        windObjective = windMaxVolume;
    }

    // Adjust volume depending on distance between players
    void AdjustMusicVolumeOnDistance()
    {
        //distanceBetweenPlayers = Mathf.Abs(players[0].transform.position.x - players[1].transform.position.x);
    }









    // FX
    // Clash sound
    public void Clash()
    {
        soundFunctions.PlaySoundFromSource(clash);
    }

    // Parry sound
    public void Parried()
    {
        soundFunctions.PlaySoundFromSource(parry);
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
}
