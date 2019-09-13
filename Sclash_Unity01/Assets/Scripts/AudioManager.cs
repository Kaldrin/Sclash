using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    // MANAGERS
    [Header("MANAGERS")]
    // Name of the GameManager to find it in the scene
    [SerializeField] string gameManagerName = "GlobalManager";
    GameManager gameManager;
    // Name of the CameraManager to find it in the scene
    [SerializeField] string cameraManagerName = "CameraArm";
    CameraManager cameraManager;



    // SOUND FUNCTIONS
    [Header("SOUND FUNCTIONS")]
    // A script that provides premade sound functions to play a little more easily with audio sources
    [SerializeField] SoundFunctions soundFunctions = null;





    // SOUND FX
    [Header("SOUND FX")]
    // Sound FXs audio sources
    [SerializeField] AudioSource clashSoundFXAudioSOurce = null;
    [SerializeField] AudioSource
        parrySoundFXAudioSource = null,
        successfulAttackSoundFXAudioSource = null,
        deathSoundFXAudioSource = null;
    // Sound FXs audio sources but with a script to play a random sound in a previously filled list
    [SerializeField] public PlayRandomSoundInList
        matchBeginsRandomSoundSource = null,
        roundBeginsRandomSoundSource = null;






    // MUSIC
    [Header("MUSIC")]
    [SerializeField] AudioSource menuMusicAudioSource = null;
    [SerializeField] AudioSource windAudioSource = null;
    [SerializeField] AudioSource[] battleMusicPhaseSources = null;

    [SerializeField] float
        musicVolumeFadeSpeed = 0.05f,
        menuMusicMaxVolume = 0.7f,
        battleMusicMaxVolume = 1f,
        windMaxVolume = 1f;
    float
        menuMusicVolumeObjective = 0.7f,
        windVolumeObjective = 0,
        volumeDifferenceToleranceToFinishVolumeInterp = 0.1f;
    float[] battleMusicsVolumeObjectives = { 0, 0, 0 };

    [HideInInspector] public bool
        adjustBattleMusicVolumeDepdendingOnPlayersDistance = true,
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

        // Chooses a music for the current session
        int randomMusicChoice = Random.Range(0, musicDataBase.musicsList.Count);
        chosenMusic = randomMusicChoice;

        // Set the default stem of the music phases audio sources
        battleMusicPhaseSources[0].clip = musicDataBase.musicsList[chosenMusic].phases[0].stems[0].stemAudio;
        battleMusicPhaseSources[1].clip = musicDataBase.musicsList[chosenMusic].phases[1].stems[0].stemAudio;
        battleMusicPhaseSources[2].clip = musicDataBase.musicsList[chosenMusic].phases[2].stems[0].stemAudio;

        // Sets the menu music volume objective at its max
        menuMusicVolumeObjective = menuMusicMaxVolume;

        // Finds the players in the scene to use their data
        FindPlayers();

        //UpdateMusicPhaseThatShouldPlayDependingOnScore();
    }

    // Update is called once per graphic frame
    void Update()
    {
        // MUSIC CHEATS
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


        // Adjusts phase 1 volume depnding on players' distance from each other
        Debug.Log(battleMusicOn);
        if (battleMusicOn)
        {
            if (currentPhase < 1)
            {
                AdjustBattleMusicVolumeDependingOnPlayersDistance();
            }
        }


        // Fades musics' volumes between them for a smooth audio rather than clear cut on / off
        InterpMusicsVolumesSmoothly();
    }







    // CHEATS
    // Cheats keys to modify music for development test purposes
    void AudioCheats()
    {
        // If cheat input to switch to next phase is pressed
        if (Input.GetKeyDown(phaseUpCheatKey))
        {
            ImmediatlySwitchPhase("Up");
        }


        // If cheat input to switch to previous phase is pressed
        if (Input.GetKeyDown(phaseDownCheatKey))
        {
            ImmediatlySwitchPhase("Down");
        }
    }








    // PLAYERS
    // Finds the players on the scene to use their data for music modification
    public void FindPlayers()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
    }








    // MUSICS ACTIVATION
    // Activates menu music
    public void MenuMusicOn()
    {
        soundFunctions.SetAudioActiveFromSource(menuMusicAudioSource, true, true);

        battleMusicOn = false;
        menuMusicVolumeObjective = menuMusicMaxVolume;
        battleMusicsVolumeObjectives[0] = 0;
        windVolumeObjective = 0;
    }  

    // Activates battle music
    public void BattleMusicOn()
    {
        soundFunctions.SetAudioActiveFromSource(battleMusicPhaseSources[0], true, false);

        battleMusicOn = true;
        menuMusicVolumeObjective = 0;
        battleMusicsVolumeObjectives[0] = battleMusicMaxVolume;
    }

    // Activates wind and deactivates other musics
    public void WindOn()
    {
        soundFunctions.SetAudioActiveFromSource(windAudioSource, true, true);

        battleMusicOn = false;
        adjustBattleMusicVolumeDepdendingOnPlayersDistance = false;

        // Set the volume objective of the wind track to its max
        windVolumeObjective = windMaxVolume;
        // Sets the volume objectives of other tracks to 0
        menuMusicVolumeObjective = 0;
        


        for (int i = 0; i < battleMusicsVolumeObjectives.Length; i++)
        {
            battleMusicsVolumeObjectives[i] = 0;
        }
    }








    // BATTLE MUSIC
    // Fades musics volumes smothly for soft transitions
    void InterpMusicsVolumesSmoothly()
    {
        // MENU MUSIC
        // Adjusts smoothly the menu music volume depending on its volume objective and checks if it's done modifying it
        if (menuMusicAudioSource.volume > menuMusicVolumeObjective)
        {
            menuMusicAudioSource.volume += -musicVolumeFadeSpeed;


            if (menuMusicAudioSource.volume <= menuMusicVolumeObjective)
            {
                menuMusicAudioSource.volume = menuMusicVolumeObjective;
                menuMusicFinishedFade = true;
            }
        }
        else if (menuMusicAudioSource.volume < menuMusicVolumeObjective)
        {
            menuMusicAudioSource.volume += musicVolumeFadeSpeed;

            if (menuMusicAudioSource.volume >= menuMusicVolumeObjective)
            {
                menuMusicAudioSource.volume = menuMusicVolumeObjective;
                menuMusicFinishedFade = true;
            }
        }
        else if (Mathf.Abs(menuMusicAudioSource.volume - menuMusicVolumeObjective) < volumeDifferenceToleranceToFinishVolumeInterp)
        {
            menuMusicFinishedFade = true;
            menuMusicAudioSource.volume = menuMusicVolumeObjective;
        }




        // BATTLE MUSIC
        // Adjusts smoothly the battle music volume depending on its volume objective and checks if it's done modifying it, for each of the battle musuc audio sources
        for (int i = 0; i < battleMusicPhaseSources.Length; i++)
        {
            // If the current volume of the battle music audio source is superior to its volume objective
            if (battleMusicPhaseSources[i].volume > battleMusicsVolumeObjectives[i])
            {
                battleMusicPhaseSources[i].volume += - musicVolumeFadeSpeed;


                if (battleMusicPhaseSources[i].volume <= battleMusicsVolumeObjectives[i])
                {
                    battleMusicPhaseSources[i].volume = battleMusicsVolumeObjectives[i];
                    battleMusicFinishedFades[i] = true;
                }
            }
            // If the current volume of the battle music audio source is inferior to its volume objective
            else if (battleMusicPhaseSources[i].volume < battleMusicsVolumeObjectives[i])
            {
                battleMusicPhaseSources[i].volume += musicVolumeFadeSpeed;


                if (battleMusicPhaseSources[i].volume >= battleMusicsVolumeObjectives[i])
                {
                    battleMusicPhaseSources[i].volume = battleMusicsVolumeObjectives[i];
                    battleMusicFinishedFades[i] = true;
                }
            }
            else if (Mathf.Abs(battleMusicPhaseSources[i].volume - battleMusicsVolumeObjectives[i]) < volumeDifferenceToleranceToFinishVolumeInterp)
            {
                battleMusicFinishedFades[i] = true;
                battleMusicPhaseSources[i].volume = battleMusicsVolumeObjectives[i];
            }
        }





        // WIND
        // Adjusts smoothly the wind track volume depending on its volume objective and checks if it's done modifying it
        // If the current volume of the wind audio source is superior to its volume objective
        if (windAudioSource.volume > windVolumeObjective)
        {
            windAudioSource.volume += - musicVolumeFadeSpeed;

            if (windAudioSource.volume <= windVolumeObjective)
            {
                windAudioSource.volume = windVolumeObjective;
                windFinishedFade = true;
            }
        }
        // If the current volume of the wind audio source is inferior to its volume objective
        else if (windAudioSource.volume < windVolumeObjective)
        {
            windAudioSource.volume += musicVolumeFadeSpeed;


            if (windAudioSource.volume >= windVolumeObjective)
            {
                windAudioSource.volume = windVolumeObjective;
                windFinishedFade = true;
            }
        }
        else if (Mathf.Abs(windAudioSource.volume - windVolumeObjective) < volumeDifferenceToleranceToFinishVolumeInterp)
        {
            windFinishedFade = true;
            windAudioSource.volume = windVolumeObjective;
        }

        /*
        //battleMusicFinishedFade = true;
        for (int i = 0; i < battleMusicFinishedFades.Length; i++)
        {
            if (!battleMusicFinishedFades[i])
            {
                battleMusicFinishedFade = false;
            }
        }
        */
    }

    // Adjusts battle music volume depending on distance between players, called in FixedUpdate
    void AdjustBattleMusicVolumeDependingOnPlayersDistance()
    {
        if (adjustBattleMusicVolumeDepdendingOnPlayersDistance)
        {
            if (currentPhase == 0)
            {
                //battleMusicsVolumeObjectives[0] = ((cameraManager.distanceBetweenPlayers - playersDistanceForVolumeLimits.y) / (playersDistanceForVolumeLimits.x - playersDistanceForVolumeLimits.y)) * battleMusicMaxVolume;
                windVolumeObjective = windMaxVolume - battleMusicsVolumeObjectives[0];
                Debug.Log(battleMusicsVolumeObjectives[0]);
            }   
        }
    }

    // Update music parameters depending on score, called by game manager on new round
    public void UpdateMusicPhaseThatShouldPlayDependingOnScore()
    {
        // Compares the scores of the players to get the highest
        int currentMaxScore = Mathf.FloorToInt(Mathf.Max(gameManager.score[0], gameManager.score[1]));


        // Modifies the phase tha will play at the next stem dependng on the score' proximity to the win score
        if (gameManager.scoreToWin <= currentMaxScore + 1)
        {
            ModifyMusicPhaseToPlayParameters(2, 0);
        }
        else if (gameManager.scoreToWin <= currentMaxScore + Mathf.FloorToInt(gameManager.scoreToWin / 2))
        {
            ModifyMusicPhaseToPlayParameters(1, 0);
        }
    }

    // Called when there's a hit, parry, etc to count the fight's intensity
    public void BattleEventIncreaseIntensity()
    {
        intensity++;
        //Debug.Log(intensity);
    }

    // Modifies music parameters for next music change
    public void ModifyMusicPhaseToPlayParameters(int phase, int stem)
    {
        shouldChangeMusicPhase = true;
        currentPhase = phase;
        currentStem = stem;
    }

    // Immediatly switches the currently playing battle music phase to the next or the previous one while keeping the timecode
    void ImmediatlySwitchPhase(string upOrDown)
    {
        // If the instruction is to pass to next phase
        if (upOrDown == "Up")
        {
            // Checks if there's a next phase
            if (currentPhase < musicDataBase.musicsList[chosenMusic].phases.Count - 1)
            {
                float stemTime = battleMusicPhaseSources[0].time;

                // Increases phase number
                ModifyMusicPhaseToPlayParameters(currentPhase + 1, currentStem);
                shouldChangeMusicPhase = true;
                UpdateCurrentlyPlayingMusicImmediatly();
                Debug.Log("Phase up");


                // Keeps the current time of the stem
                for (int i = 0; i < battleMusicPhaseSources.Length; i++)
                {
                    battleMusicPhaseSources[i].time = stemTime;
                }
            }
            // There's no next phase
            else
            {
                Debug.Log("Can't phase up");
            }
        }
        // If the instruction is to pass to previous phase
        else if (upOrDown == "Down")
        {
            // Checks if there's a previous phase
            if (currentPhase > 0)
            {
                float stemTime = battleMusicPhaseSources[0].time;


                ModifyMusicPhaseToPlayParameters(currentPhase - 1, currentStem);
                shouldChangeMusicPhase = true;
                UpdateCurrentlyPlayingMusicImmediatly();
                Debug.Log("Phase down");


                // Keeps the current time of the stem
                for (int i = 0; i < battleMusicPhaseSources.Length; i++)
                {
                    battleMusicPhaseSources[i].time = stemTime;
                }
            }
            // There's no previous phase
            else
            {
                Debug.Log("Can't phase down");
            }
        }
    }

    // Applies music changes immediatly to the audio sources
    void UpdateCurrentlyPlayingMusicImmediatly()
    {
        // Changes the music phase
        if (shouldChangeMusicPhase)
        {
            shouldChangeMusicPhase = false;

            //battleMusicPhase1Source.clip = nextPhaseMusic;
            Debug.Log(currentPhase);
            battleMusicsVolumeObjectives[currentPhase] = battleMusicMaxVolume;

            for (int i = 0; i < battleMusicPhaseSources.Length; i++)
            {
                
                battleMusicPhaseSources[i].clip = musicDataBase.musicsList[chosenMusic].phases[i].stems[currentStem].stemAudio;
                
                battleMusicFinishedFades[i] = false;

                if ( i != currentPhase)
                {
                    battleMusicsVolumeObjectives[i] = 0;
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

    // Resets music parameters for next match
    public void ResetBattleMusicPhase()
    {
        // Resets phase and stem to 0
        ModifyMusicPhaseToPlayParameters(0, 0);

        int randomMusicChoice = Random.Range(0, musicDataBase.musicsList.Count);
        chosenMusic = randomMusicChoice;
        battleMusicPhaseSources[0].clip = musicDataBase.musicsList[chosenMusic].phases[currentPhase].stems[currentStem].stemAudio;
    }








    // SOUND FX
    // Plays clash sound FX
    public void Clash()
    {
        soundFunctions.PlaySoundFromSource(clashSoundFXAudioSOurce);
    }

    // Plays successful parry sound FX
    public void Parried()
    {
        soundFunctions.PlaySoundFromSource(parrySoundFXAudioSource);
    }

    // Triggers successful attack sound FX play coroutine
    public void SuccessfulAttack()
    {
        StartCoroutine(SuccessfulAttackCoroutine());
    }

    // Plays successful attack sound FX
    IEnumerator SuccessfulAttackCoroutine()
    {
        soundFunctions.PlaySoundFromSource(successfulAttackSoundFXAudioSource);

        yield return new WaitForSeconds(0f);
        soundFunctions.PlaySoundFromSource(deathSoundFXAudioSource);
    }
}
