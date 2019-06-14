using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // SOUND FUNCTIONS7
    [Header("Sound functions")]
    [SerializeField] SoundFunctions soundFunctions = null;



    // FX
    [Header("FX")]
    // Audio sources
    [SerializeField] AudioSource successfulAttack = null;
    [SerializeField] AudioSource
        death = null,
        parried = null;
    [SerializeField] PlayRandomSoundInList clashRandomSoundScript = null;



    // MUSIC
    [Header("Music")]
    // Audio sources
    [SerializeField] AudioSource menuMusicSource = null;
    [SerializeField] AudioSource battleMusicSource = null;

    float menuMusicObjective = 1;
    float battleMusicObjective = 0;
    [SerializeField] float musicFadeSpeed = 0.001f;
    float volumeComparisonTolerance = 0.1f;

    bool battleOn = false;
    bool menuMusicFinishedFade = false;
    bool battleMusicFinishedFade = false;
    
    [SerializeField] Vector2 battleMusicVolumeDistanceBounds = new Vector2(5, 20);
    [SerializeField] Vector2 battleMusicVolumeBounds = new Vector2(1, 0);



    // VOICE
    [Header("Voice")]
    [SerializeField] AudioSource deathVoice;
    [SerializeField] AudioSource attackVoice;
    [SerializeField] AudioSource introVoice;



    // PLAYERS
    [Header("Players")]
    GameObject[] players;
    float distanceBetweenPlayers = 0;
    












    // BASE FUNCTIONS
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






    // PLAYERS
    // Find the players in scene
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
        soundFunctions.SetAudioActiveFromSource(menuMusicSource, true);

        menuMusicObjective = 1;
        battleMusicObjective = 0;

        ActivateMusicFade();
    }  

    public void BattleMusicOn()
    {
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
        //soundFunctions.PlaySoundFromSource(clash);
        clashRandomSoundScript.play = true;
    }

    public void SuccessfulAttack()
    {
        soundFunctions.PlaySoundFromSource(successfulAttack);
    }

    public void Parried()
    {
        soundFunctions.PlaySoundFromSource(parried);
    }

    public void Death()
    {
        soundFunctions.PlaySoundFromSource(death);
    }
}
