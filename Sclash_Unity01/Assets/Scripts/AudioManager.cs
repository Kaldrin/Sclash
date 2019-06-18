using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // SOUND FUNCTIONS
    [Header("Sound functions")]
    [SerializeField] SoundFunctions soundFunctions = null;





    // FX
    // Audio sources
    [Header("FX")]
    [SerializeField] AudioSource clash = null;
    [SerializeField] AudioSource
        parry = null,
        successfulAttack = null,
        death = null;




    // MUSIC
    [Header("SMusic")]
    [SerializeField] AudioSource menuMusicSource = null;
    [SerializeField] AudioSource battleMusicSource = null;
    [SerializeField]
    float
        menuMusicMaxVolume = 0.7f,
        battleMusicMaxVolume = 1f;

    float menuMusicObjective = 0.7f;
    float battleMusicObjective = 0;
    bool battleOn = false;
    // bool fadeMusic = false;
    bool menuMusicFinishedFade = false;
    bool battleMusicFinishedFade = false;
    [SerializeField] float musicFadeSpeed = 0.001f;
    float volumeComparisonTolerance = 0.1f;



    // VOICE
    [Header("Voice")]
    [SerializeField] AudioSource deathVoice;
    [SerializeField] AudioSource introVoice;



    // PLAYERS
    [Header("Players")]
    [SerializeField] Vector2 battleMusicVolumeDistanceBounds = new Vector2(5, 20);
    [SerializeField] Vector2 battleMusicVolumeBounds = new Vector2(1, 0);

    GameObject[] players;
    float distanceBetweenPlayers = 0;








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
    void ActivateMusicFade()
    {
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
            battleMusicFinishedFade = false;
            menuMusicFinishedFade = false;

            battleMusicSource.volume = battleMusicObjective;
            menuMusicSource.volume = menuMusicObjective;
        }
    }

    public void MenuMusicOn()
    {
        soundFunctions.SetAudioActiveFromSource(menuMusicSource, true);

        menuMusicObjective = menuMusicMaxVolume;
        battleMusicObjective = 0;

        ActivateMusicFade();
    }  

    public void BattleMusicOn()
    {
        soundFunctions.SetAudioActiveFromSource(battleMusicSource, true);

        battleOn = true;
        menuMusicObjective = 0;
        battleMusicObjective = battleMusicMaxVolume;

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
