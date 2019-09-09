using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    // MANAGERS
    [Header("MANAGERS")]
    [SerializeField] string gameManagerName = "GlobalManager";
    GameManager gameManager;
    [SerializeField] string cameraManagerName = "CameraArm";
    CameraManager cameraManager;



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
    [SerializeField] AudioSource windSource = null;
    [SerializeField] AudioSource[] battleMusicPhaseSources = null;
        

    [SerializeField] float
        musicFadeSpeed = 0.001f,
        menuMusicMaxVolume = 0.7f,
        battleMusicMaxVolume = 1f,
        windMaxVolume = 1f;
    float
        menuMusicObjective = 0.7f,
        windObjective = 0,
        volumeComparisonTolerance = 0.1f;
    float[] battleMusicsObjectives = { 0, 0, 0 };

    [HideInInspector] public bool
        adjustVolumeOnDistance = false,
        battleMusicOn = false;
    bool
        menuMusicFinishedFade = false,
        battleMusicFinishedFade = false,
        windFinishedFade = false,
        shouldChangeMusicPhase = false;
    bool[] battleMusicFinishedFades = { false, false, false };

    int
        randomMusicChoice = 0,
        chosenMusic = 0,
        currentPhase = 0,
        currentStem = 0;

    [SerializeField] Vector2 playersDistanceForVolumeLimits = new Vector2(25, 10);
    AudioClip nextPhaseMusic = null;

    




    // MUSIC DATA
    [SerializeField] MusicsDatabase musicDataBase = null;







    // VOICE
    [Header("VOICE")]
    [SerializeField] AudioSource deathVoice;
    [SerializeField] AudioSource introVoice;






    // PLAYERS
    [Header("PLAYERS")]
    [SerializeField] Vector2 battleMusicVolumeDistanceBounds = new Vector2(5, 20);
    [SerializeField] Vector2 battleMusicVolumeBounds = new Vector2(1, 0);

    GameObject[] players;
    int intensity = 0;
    //float distanceBetweenPlayers = 0;





    // CHEATS FOR DEVELOPMENT PURPOSES
    [Header("CHEATS")]
    [SerializeField] KeyCode phaseUpCheatKey = KeyCode.Alpha1;
    [SerializeField] KeyCode phaseDownCheatKey = KeyCode.Alpha2;
    











    // BASE FUNCTIONS
    // Start is called before the first frame update
    void Start()
    {
        // Get managers
        gameManager = GameObject.Find(gameManagerName).GetComponent<GameManager>();
        cameraManager = GameObject.Find(cameraManagerName).GetComponent<CameraManager>();

        int randomMusicChoice = Random.Range(0, musicDataBase.musicsList.Count);
        chosenMusic = randomMusicChoice;
        battleMusicPhaseSources[0].clip = musicDataBase.musicsList[chosenMusic].phases[0].stems[0].stemAudio;
        battleMusicPhaseSources[1].clip = musicDataBase.musicsList[chosenMusic].phases[1].stems[0].stemAudio;
        battleMusicPhaseSources[2].clip = musicDataBase.musicsList[chosenMusic].phases[2].stems[0].stemAudio;

        menuMusicObjective = menuMusicMaxVolume;
        FindPlayers();
        UpdatePhaseMusic();
    }

    // Update is called once per frame
    void Update()
    {
        // Adjusts phase 1 volume depnding on players' distance
        
        if (battleMusicOn)
        {
            if (currentPhase < 1)
            {
                //AdjustMusicVolumeOnDistance();
            }   
        }
        

        // CHEATS
        if (gameManager.cheatCodes)
            AudioCheats();
    }

    // FixedUpdate is called 50 times per frame
    private void FixedUpdate()
    {
        // Applies music changes when stem finished playing
        if (!battleMusicPhaseSources[currentPhase].isPlaying)
        {
            UpdateCurrentlyPlayingMusicImmediatly();
        }


        // Fades musics' volumes between them for a smooth audio rather than clear cut on / off
        FadeMusic();
    }





    // CHEATS
    void AudioCheats()
    {
        if (Input.GetKeyDown(phaseUpCheatKey))
        {
            if (currentPhase < 2)
            {
                float stemTime = battleMusicPhaseSources[0].time;


                ModifyMusicPhase(currentPhase + 1, 0);
                shouldChangeMusicPhase = true;
                UpdateCurrentlyPlayingMusicImmediatly();
                Debug.Log("Phase up");


                for (int i = 0; i < battleMusicPhaseSources.Length; i++)
                {
                    battleMusicPhaseSources[i].time = stemTime;
                }
            }
            else
            {
                Debug.Log("Can't phase up");
            }
        }


        if (Input.GetKeyDown(phaseDownCheatKey))
        {
            if (currentPhase > 0)
            {
                float stemTime = battleMusicPhaseSources[0].time;


                ModifyMusicPhase(currentPhase - 1, 0);
                shouldChangeMusicPhase = true;
                UpdateCurrentlyPlayingMusicImmediatly();
                Debug.Log("Phase down");


                for (int i = 0; i < battleMusicPhaseSources.Length; i++)
                {
                    battleMusicPhaseSources[i].time = stemTime;
                }
            }
            else
            {
                Debug.Log("Can't phase down");
            }
        }
    }








    // PLAYERS
    public void FindPlayers()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    public void BattleEvent()
    {
        intensity++;
        //Debug.Log(intensity);
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
            }
        }
        else if (menuMusicSource.volume < menuMusicObjective)
        {
            menuMusicSource.volume += musicFadeSpeed;

            if (menuMusicSource.volume >= menuMusicObjective)
            {
                menuMusicSource.volume = menuMusicObjective;
                menuMusicFinishedFade = true;
            }
        }
        else if (Mathf.Abs(menuMusicSource.volume - menuMusicObjective) < volumeComparisonTolerance)
        {
            menuMusicFinishedFade = true;
            menuMusicSource.volume = menuMusicObjective;
        }




        // Battle music
        for (int i = 0; i < battleMusicPhaseSources.Length; i++)
        {
            if (battleMusicPhaseSources[i].volume > battleMusicsObjectives[i])
            {
                battleMusicPhaseSources[i].volume += -musicFadeSpeed;


                if (battleMusicPhaseSources[i].volume <= battleMusicsObjectives[i])
                {
                    battleMusicPhaseSources[i].volume = battleMusicsObjectives[i];
                    battleMusicFinishedFades[i] = true;
                }
            }
            else if (battleMusicPhaseSources[i].volume < battleMusicsObjectives[i])
            {
                battleMusicPhaseSources[i].volume += musicFadeSpeed;


                if (battleMusicPhaseSources[i].volume >= battleMusicsObjectives[i])
                {
                    battleMusicPhaseSources[i].volume = battleMusicsObjectives[i];
                    battleMusicFinishedFades[i] = true;
                }
            }
            else if (Mathf.Abs(battleMusicPhaseSources[i].volume - battleMusicsObjectives[i]) < volumeComparisonTolerance)
            {
                battleMusicFinishedFades[i] = true;
                battleMusicPhaseSources[i].volume = battleMusicsObjectives[i];
            }
        }
        




        // Wind
        if (windSource.volume > windObjective)
        {
            windSource.volume += -musicFadeSpeed;

            if (windSource.volume <= windObjective)
            {
                windSource.volume = windObjective;
                windFinishedFade = true;
            }
        }
        else if (windSource.volume < windObjective)
        {
            windSource.volume += musicFadeSpeed;


            if (windSource.volume >= windObjective)
            {
                windSource.volume = windObjective;
                windFinishedFade = true;
            }
        }
        else if (Mathf.Abs(windSource.volume - windObjective) < volumeComparisonTolerance)
        {
            windFinishedFade = true;
            windSource.volume = windObjective;
        }


        battleMusicFinishedFade = true;
        for (int i = 0; i < battleMusicFinishedFades.Length; i++)
        {
            if (!battleMusicFinishedFades[i])
            {
                battleMusicFinishedFade = false;
            }
        }

        if (menuMusicFinishedFade && battleMusicFinishedFade && windFinishedFade)
        {
            battleMusicFinishedFade = false;
            menuMusicFinishedFade = false;
            windFinishedFade = false;

            battleMusicPhaseSources[0].volume = battleMusicsObjectives[0];
            menuMusicSource.volume = menuMusicObjective;
            windSource.volume = windObjective;
        }
    }

    // Activate menu music
    public void MenuMusicOn()
    {
        soundFunctions.SetAudioActiveFromSource(menuMusicSource, true, true);

        battleMusicOn = false;
        menuMusicObjective = menuMusicMaxVolume;
        battleMusicsObjectives[0] = 0;
        windObjective = 0;

        //ActivateMusicFade();
    }  

    // Activate battle music
    public void BattleMusicOn()
    {
        soundFunctions.SetAudioActiveFromSource(battleMusicPhaseSources[0], true, false);

        battleMusicOn = true;
        menuMusicObjective = 0;
        battleMusicsObjectives[0] = battleMusicMaxVolume;
        //windObjective = 0;

        //ActivateMusicFade();
    }

    // Activate wind
    public void WindOn()
    {
        soundFunctions.SetAudioActiveFromSource(windSource, true, true);

        battleMusicOn = false;

        menuMusicObjective = 0;
        windObjective = windMaxVolume;


        for (int i = 0; i <battleMusicsObjectives.Length; i++)
        {
            battleMusicsObjectives[i] = 0;
        }
    }

    // Adjust volume depending on distance between players
    void AdjustMusicVolumeOnDistance()
    {
        if (adjustVolumeOnDistance)
        {
            if (currentPhase == 0)
                battleMusicsObjectives[0] = ((cameraManager.distanceBetweenPlayers - playersDistanceForVolumeLimits.y) / (playersDistanceForVolumeLimits.x - playersDistanceForVolumeLimits.y)) * battleMusicMaxVolume;
        }
    }

    // Update music parameters depending on score, called by game manager on new round
    public void UpdatePhaseMusic()
    {
        int currentMaxScore = Mathf.FloorToInt(Mathf.Max(gameManager.score[0], gameManager.score[1]));

        if (gameManager.scoreToWin <= currentMaxScore + 1)
        {
            //nextPhaseMusic = musicDataBase.musicsList[chosenMusic].phase3;
            ModifyMusicPhase(2, 0);
            //battleMusicPhase1Source.clip = musicDataBase.musicsList[chosenMusic].phase3;
            //battleMusicPhase1Source.Play();
        }
        else if (gameManager.scoreToWin <= currentMaxScore + Mathf.FloorToInt(gameManager.scoreToWin / 2))
        {
            ModifyMusicPhase(1, 0);
        }
    }

    // Modifies music parameters for next musc changes
    public void ModifyMusicPhase(int phase, int stem)
    {
        shouldChangeMusicPhase = true;
        currentPhase = phase;
        currentStem = stem;
    }

    // Resets music parameters for next match
    public void ResetMusicPhase()
    {
        currentPhase = 0;
        currentStem = 0;

        int randomMusicChoice = Random.Range(0, musicDataBase.musicsList.Count);
        chosenMusic = randomMusicChoice;
        battleMusicPhaseSources[0].clip = musicDataBase.musicsList[chosenMusic].phases[currentPhase].stems[currentStem].stemAudio;
    }

    // Applies music changes immediatly
    void UpdateCurrentlyPlayingMusicImmediatly()
    {
        // Changes the music phase
        if (shouldChangeMusicPhase)
        {
            shouldChangeMusicPhase = false;

            //battleMusicPhase1Source.clip = nextPhaseMusic;
            Debug.Log(currentPhase);
            battleMusicsObjectives[currentPhase] = battleMusicMaxVolume;

            for (int i = 0; i < battleMusicPhaseSources.Length; i++)
            {
                
                battleMusicPhaseSources[i].clip = musicDataBase.musicsList[chosenMusic].phases[i].stems[currentStem].stemAudio;
                
                battleMusicFinishedFades[i] = false;

                if ( i != currentPhase)
                {
                    battleMusicsObjectives[i] = 0;
                }
            }
        }
        else
        {
            /*
            int randomStem = 0;


            do
            {
                randomStem = Random.Range(0, musicDataBase.musicsList[chosenMusic].phases[currentPhase].stems.Count);
            }
            while (randomStem == currentStem);
            */

            int nextStemThatConnectsIndex = Random.Range(0, musicDataBase.musicsList[chosenMusic].phases[currentPhase].stems[currentStem].stemsThatConnect.Count);
            int nextStem = musicDataBase.musicsList[chosenMusic].phases[currentPhase].stems[currentStem].stemsThatConnect[nextStemThatConnectsIndex];


            currentStem = nextStem;
            battleMusicPhaseSources[currentPhase].clip = musicDataBase.musicsList[chosenMusic].phases[currentPhase].stems[currentStem].stemAudio;
        }

        for (int i = 0; i < battleMusicPhaseSources.Length; i++)
        {
            battleMusicPhaseSources[i].Play();
            battleMusicPhaseSources[i].time = 0;
        }
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
