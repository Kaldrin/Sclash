using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    // MANAGERS
    [Header("MANAGERS")]
    // Name of the GameManager to find it in the scene
    [SerializeField] GameManager gameManager;
    // Name of the CameraManager to find it in the scene
    [SerializeField] CameraManager cameraManager;



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
    [SerializeField] AudioSource
        windAudioSource = null,
        winAudioSource = null;
    [SerializeField] public AudioSource[]
        battleMusicPhaseSources = null,
        battleMusicStrikesSources = null;

    [SerializeField] float
        musicVolumeFadeSpeed = 0.05f,
        menuMusicMaxVolume = 0.7f,
        battleMusicMaxVolume = 1f,
        windMaxVolume = 1f,
        battleMusicStrikesMaxVolume = 0.7f,
        decreaseBattleIntensityEveryDuration = 5f;
    float
        menuMusicVolumeObjective = 0.7f,
        windVolumeObjective = 0,
        volumeDifferenceToleranceToFinishVolumeInterp = 0.1f,
        decreaseBattleIntensityLoopStartTime = 0f;
    float[]
        battleMusicsVolumeObjectives = { 0, 0, 0 },
        battleMusicsStrikesVolumeObjectives = { 0, 0, 0 };

    [HideInInspector] public bool
        adjustBattleMusicVolumeDepdendingOnPlayersDistance = true,
        battleMusicOn = false;
    bool shouldChangeMusicPhase = false;

    [SerializeField]
    int
        battleIntensityLevelForPhase2 = 3,
        battleIntensityLevelForPhase3 = 6;
    public int currentPhase = 0;
    int
        chosenMusic = 0,
        currentStem = 0,
        battleIntensity = 0,
        lastIntensityLevel = 0;

    
        

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
    //float distanceBetweenPlayers = 0;





    // CHEATS FOR DEVELOPMENT PURPOSES
    [Header("CHEATS")]
    [SerializeField] KeyCode phaseUpCheatKey = KeyCode.Alpha1;
    [SerializeField] KeyCode phaseDownCheatKey = KeyCode.Alpha2;
    
















    // BASE FUNCTIONS
    // Start is called before the first frame update
    void Start()
    {
        // Chooses a music for the current session
        int randomMusicChoice = Random.Range(0, musicDataBase.musicsList.Count);
        chosenMusic = randomMusicChoice;

        // Set the default stem of the music phases audio sources
        battleMusicPhaseSources[0].clip = musicDataBase.musicsList[chosenMusic].phases[0].stems[0].stemAudio;
        battleMusicPhaseSources[1].clip = musicDataBase.musicsList[chosenMusic].phases[1].stems[0].stemAudio;
        battleMusicPhaseSources[2].clip = musicDataBase.musicsList[chosenMusic].phases[2].stems[0].stemAudio;

        battleMusicStrikesSources[0].clip = musicDataBase.musicsList[chosenMusic].phases[0].stems[0].stemStrikesAudio;
        battleMusicStrikesSources[1].clip = musicDataBase.musicsList[chosenMusic].phases[1].stems[0].stemStrikesAudio;
        battleMusicStrikesSources[2].clip = musicDataBase.musicsList[chosenMusic].phases[2].stems[0].stemStrikesAudio;

        winAudioSource.clip = musicDataBase.musicsList[chosenMusic].winAudio;


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
        if (!battleMusicPhaseSources[currentPhase].isPlaying && gameManager.gameStarted && battleMusicOn)
        {
            UpdateCurrentlyPlayingMusicImmediatly();
        }


        // Adjusts phase 1 volume depending on players' distance from each other
        if (battleMusicOn)
        {
            if (currentPhase < 1)
            {
                //Debug.Log(adjustBattleMusicVolumeDepdendingOnPlayersDistance);
                AdjustBattleMusicVolumeDependingOnPlayersDistance();
            }
        }


        // Fades musics' volumes between them for a smooth audio rather than clear cut on / off
        InterpMusicsVolumesSmoothly();


        // If no battle event increases intensity in a laps of time, decreases the music's intensity
        DecreaseIntensityWithTime();


        /*
        Debug.Log(battleMusicsVolumeObjectives[0]);
        Debug.Log(battleMusicsStrikesVolumeObjectives[0]);
        */
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
    public void ActivateMenuMusic()
    {
        soundFunctions.SetAudioActiveFromSource(menuMusicAudioSource, true, true);
        soundFunctions.SetAudioActiveFromSource(winAudioSource, false, false);

        battleMusicOn = false;
        adjustBattleMusicVolumeDepdendingOnPlayersDistance = false;
        menuMusicVolumeObjective = menuMusicMaxVolume;


        for (int i = 0; i < battleMusicsVolumeObjectives.Length; i++)
        {
            battleMusicsVolumeObjectives[i] = 0;
            battleMusicsStrikesVolumeObjectives[i] = 0;
        }

        
        

        windVolumeObjective = 0;
    }  

    // Activates battle music
    public void ActivateBattleMusic()
    {
        soundFunctions.SetAudioActiveFromSource(battleMusicPhaseSources[0], true, false);
        soundFunctions.SetAudioActiveFromSource(battleMusicStrikesSources[0], true, false);

        battleMusicOn = true;
        adjustBattleMusicVolumeDepdendingOnPlayersDistance = true;

        windVolumeObjective = 0;
        menuMusicVolumeObjective = 0;


        battleMusicsVolumeObjectives[0] = battleMusicMaxVolume;
        battleMusicsStrikesVolumeObjectives[0] = battleMusicStrikesMaxVolume;
    }

    // Activates wind and deactivates other musics
    public void ActivateWind()
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
            battleMusicsStrikesVolumeObjectives[i] = 0;
        }
    }

    public void ActivateWinMusic()
    {
        //soundFunctions.SetAudioActiveFromSource(winAudioSource, true, true);
        soundFunctions.PlaySoundFromSource(winAudioSource);

        battleMusicOn = false;
        adjustBattleMusicVolumeDepdendingOnPlayersDistance = false;
        battleIntensity = 0;


        // Set the volume objective of the wind track to its max
        //windVolumeObjective = windMaxVolume;
        // Sets the volume objectives of other tracks to 0
        menuMusicVolumeObjective = 0;


        for (int i = 0; i < battleMusicsVolumeObjectives.Length; i++)
        {
            battleMusicsVolumeObjectives[i] = 0;
            battleMusicsStrikesVolumeObjectives[i] = 0;
            battleMusicPhaseSources[i].Stop();
        }
    }







    // BATTLE MUSIC
    // Fades musics volumes smothly for soft transitions
    void InterpMusicsVolumesSmoothly()
    {
        // MENU MUSIC
        // Adjusts smoothly the menu music volume depending on its volume objective and checks if it's done modifying it
        if (menuMusicAudioSource.volume != menuMusicVolumeObjective)
        {
            if (menuMusicAudioSource.volume > menuMusicVolumeObjective)
            {
                menuMusicAudioSource.volume += -musicVolumeFadeSpeed;


                if (menuMusicAudioSource.volume <= menuMusicVolumeObjective)
                {
                    menuMusicAudioSource.volume = menuMusicVolumeObjective;
                }
            }
            else if (menuMusicAudioSource.volume < menuMusicVolumeObjective)
            {
                menuMusicAudioSource.volume += musicVolumeFadeSpeed;

                if (menuMusicAudioSource.volume >= menuMusicVolumeObjective)
                {
                    menuMusicAudioSource.volume = menuMusicVolumeObjective;
                }
            }
            else if (Mathf.Abs(menuMusicAudioSource.volume - menuMusicVolumeObjective) < volumeDifferenceToleranceToFinishVolumeInterp)
            {
                menuMusicAudioSource.volume = menuMusicVolumeObjective;
            }
        }
        



        // BATTLE MUSIC
        // Adjusts smoothly the battle music volume depending on its volume objective and checks if it's done modifying it, for each of the battle musuc audio sources
        for (int i = 0; i < battleMusicPhaseSources.Length; i++)
        {
            if (battleMusicPhaseSources[i].volume != battleMusicsVolumeObjectives[i])
            {
                // If the current volume of the battle music audio source is superior to its volume objective
                if (battleMusicPhaseSources[i].volume > battleMusicsVolumeObjectives[i])
                {
                    battleMusicPhaseSources[i].volume += -musicVolumeFadeSpeed;


                    if (battleMusicPhaseSources[i].volume <= battleMusicsVolumeObjectives[i])
                    {
                        battleMusicPhaseSources[i].volume = battleMusicsVolumeObjectives[i];
                    }
                }
                // If the current volume of the battle music audio source is inferior to its volume objective
                else if (battleMusicPhaseSources[i].volume < battleMusicsVolumeObjectives[i])
                {
                    battleMusicPhaseSources[i].volume += musicVolumeFadeSpeed;


                    if (battleMusicPhaseSources[i].volume >= battleMusicsVolumeObjectives[i])
                    {
                        battleMusicPhaseSources[i].volume = battleMusicsVolumeObjectives[i];
                    }
                }
                else if (Mathf.Abs(battleMusicPhaseSources[i].volume - battleMusicsVolumeObjectives[i]) < volumeDifferenceToleranceToFinishVolumeInterp)
                {
                    battleMusicPhaseSources[i].volume = battleMusicsVolumeObjectives[i];
                }
            }
        }




        // BATTLE MUSIC STRIKES AUDIO
        // Adjusts smoothly the battle music strikes audio volume depending on its volume objective and checks if it's done modifying it, for each of the battle musuc audio sources
        for (int i = 0; i < battleMusicPhaseSources.Length; i++)
        {
            if (battleMusicStrikesSources[i].volume != battleMusicsStrikesVolumeObjectives[i])
            {
                // If the current volume of the battle music audio source is superior to its volume objective
                if (battleMusicStrikesSources[i].volume > battleMusicsStrikesVolumeObjectives[i])
                {
                    battleMusicStrikesSources[i].volume += -musicVolumeFadeSpeed;


                    if (battleMusicStrikesSources[i].volume <= battleMusicsVolumeObjectives[i])
                    {
                        battleMusicStrikesSources[i].volume = battleMusicsStrikesVolumeObjectives[i];
                    }
                }
                // If the current volume of the battle music audio source is inferior to its volume objective
                else if (battleMusicStrikesSources[i].volume < battleMusicsStrikesVolumeObjectives[i])
                {
                    battleMusicStrikesSources[i].volume += musicVolumeFadeSpeed;


                    if (battleMusicStrikesSources[i].volume >= battleMusicsStrikesVolumeObjectives[i])
                    {
                        battleMusicStrikesSources[i].volume = battleMusicsStrikesVolumeObjectives[i];
                    }
                }
                else if (Mathf.Abs(battleMusicStrikesSources[i].volume - battleMusicsStrikesVolumeObjectives[i]) < volumeDifferenceToleranceToFinishVolumeInterp)
                {
                    battleMusicStrikesSources[i].volume = battleMusicsStrikesVolumeObjectives[i];
                }
            }
        }





        // WIND
        // Adjusts smoothly the wind track volume depending on its volume objective and checks if it's done modifying it
        if (windAudioSource.volume != windVolumeObjective)
        {
            // If the current volume of the wind audio source is superior to its volume objective
            if (windAudioSource.volume > windVolumeObjective)
            {
                windAudioSource.volume += -musicVolumeFadeSpeed;

                if (windAudioSource.volume <= windVolumeObjective)
                {
                    windAudioSource.volume = windVolumeObjective;
                }
            }
            // If the current volume of the wind audio source is inferior to its volume objective
            else if (windAudioSource.volume < windVolumeObjective)
            {
                windAudioSource.volume += musicVolumeFadeSpeed;


                if (windAudioSource.volume >= windVolumeObjective)
                {
                    windAudioSource.volume = windVolumeObjective;
                }
            }
            else if (Mathf.Abs(windAudioSource.volume - windVolumeObjective) < volumeDifferenceToleranceToFinishVolumeInterp)
            {
                windAudioSource.volume = windVolumeObjective;
            }
        }
    }

    // Adjusts battle music volume depending on distance between players, called in FixedUpdate
    void AdjustBattleMusicVolumeDependingOnPlayersDistance()
    {
        if (adjustBattleMusicVolumeDepdendingOnPlayersDistance)
        {
            if (currentPhase == 0)
            {
                /*
                battleMusicsVolumeObjectives[0] = ((cameraManager.distanceBetweenPlayers - playersDistanceForVolumeLimits.y) / (playersDistanceForVolumeLimits.x - playersDistanceForVolumeLimits.y)) * battleMusicMaxVolume;


                if (battleMusicsVolumeObjectives[0] > battleMusicMaxVolume)
                    battleMusicsVolumeObjectives[0] = battleMusicMaxVolume;
                else if (battleMusicsVolumeObjectives[0] < 0)
                    battleMusicsVolumeObjectives[0] = 0;
                    */


                //windVolumeObjective = windMaxVolume - battleMusicsVolumeObjectives[0];
                //Debug.Log(battleMusicsVolumeObjectives[0]);

                //battleMusicsVolumeObjectives[0] = battleMusicMaxVolume;
            }
            

            for (int i = 0; i < battleMusicsStrikesVolumeObjectives.Length; i++)
            {
                float tempVolume = ((cameraManager.distanceBetweenPlayers - playersDistanceForVolumeLimits.y) / (playersDistanceForVolumeLimits.x - playersDistanceForVolumeLimits.y)) * battleMusicMaxVolume;
                

                if (tempVolume < 0)
                    tempVolume = 0;
                else if (tempVolume > battleMusicStrikesMaxVolume)
                    tempVolume = battleMusicStrikesMaxVolume;


                //battleMusicsStrikesVolumeObjectives[i] = battleMusicStrikesMaxVolume - tempVolume;
                battleMusicsStrikesVolumeObjectives[i] = tempVolume;
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
            ModifyMusicPhaseToPlayParameters(2, 0, true);
        }
        /*
        else if (gameManager.scoreToWin <= currentMaxScore + Mathf.FloorToInt(gameManager.scoreToWin / 2))
        {
            ModifyMusicPhaseToPlayParameters(1, 0);
        }
        */
    }

    // Called when there's a hit, parry, etc to count the fight's intensity
    public void BattleEventIncreaseIntensity()
    {
        lastIntensityLevel = battleIntensity;
        battleIntensity++;


        UpdateMusicPhaseImmediatlyDependingOnBattleIntensity();


        decreaseBattleIntensityLoopStartTime = Time.time;
    }

    void DecreaseIntensityWithTime()
    {
        if (battleIntensity > 0)
        {
            if (Time.time - decreaseBattleIntensityLoopStartTime >= decreaseBattleIntensityEveryDuration)
            {
                lastIntensityLevel = battleIntensity;
                battleIntensity--;


                UpdateMusicPhaseImmediatlyDependingOnBattleIntensity();


                decreaseBattleIntensityLoopStartTime = Time.time;
            }
        }
    }

    // Updates immediatly the currently playing music phase depending on the level of intensity in the battle
    void UpdateMusicPhaseImmediatlyDependingOnBattleIntensity()
    {
        int currentMaxScore = Mathf.FloorToInt(Mathf.Max(gameManager.score[0], gameManager.score[1]));

        if (currentMaxScore < gameManager.scoreToWin - 1 || gameManager.scoreToWin <= 1)
        {
            // Modifies the phase that will play at the next stem depending on the battle intensity
            if (currentPhase == 2)
            {
                if (battleIntensity < battleIntensityLevelForPhase3)
                {
                    ModifyMusicPhaseToPlayParameters(1, 0, true);
                    //ImmediatlySwitchChosenPhase(1);
                }
            }
            else if (currentPhase == 1)
            {
                if (battleIntensity < battleIntensityLevelForPhase2)
                {
                    ModifyMusicPhaseToPlayParameters(0, 0, true);
                    //ImmediatlySwitchChosenPhase(0);
                }
                else if (battleIntensity >= battleIntensityLevelForPhase3)
                {
                    ImmediatlySwitchChosenPhase(2);
                }
            }
            else if (currentPhase == 0)
            {
                if (battleIntensity >= battleIntensityLevelForPhase2)
                {
                    ImmediatlySwitchChosenPhase(1);
                }
            }
        }
    }

    // Modifies music parameters for next music change
    public void ModifyMusicPhaseToPlayParameters(int phase, int stem, bool shouldPlayAfter)
    {
        shouldChangeMusicPhase = shouldPlayAfter;
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
                ModifyMusicPhaseToPlayParameters(currentPhase + 1, currentStem, true);
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


                ModifyMusicPhaseToPlayParameters(currentPhase - 1, currentStem, true);
                shouldChangeMusicPhase = true;
                UpdateCurrentlyPlayingMusicImmediatly();


                // Keeps the current time of the stem
                for (int i = 0; i < battleMusicPhaseSources.Length; i++)
                {
                    battleMusicPhaseSources[i].time = stemTime;
                }
            }
            // There's no previous phase
            else
            {
            }
        }
    }

    void ImmediatlySwitchChosenPhase(int phase)
    {

        float stemTime = battleMusicPhaseSources[0].time;

        // Increases phase number
        ModifyMusicPhaseToPlayParameters(phase, currentStem, true);
        shouldChangeMusicPhase = true;
        UpdateCurrentlyPlayingMusicImmediatly();


        // Keeps the current time of the stem
        for (int i = 0; i < battleMusicPhaseSources.Length; i++)
        {
            battleMusicPhaseSources[i].time = stemTime;
            battleMusicStrikesSources[i].time = stemTime;
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
            battleMusicsVolumeObjectives[currentPhase] = battleMusicMaxVolume;
            battleMusicsStrikesVolumeObjectives[currentPhase] = battleMusicMaxVolume;

            for (int i = 0; i < battleMusicPhaseSources.Length; i++)
            {
                
                battleMusicPhaseSources[i].clip = musicDataBase.musicsList[chosenMusic].phases[i].stems[currentStem].stemAudio;
                battleMusicStrikesSources[i].clip = musicDataBase.musicsList[chosenMusic].phases[i].stems[currentStem].stemStrikesAudio;

                if ( i != currentPhase)
                {
                    battleMusicsVolumeObjectives[i] = 0;
                    battleMusicsStrikesVolumeObjectives[i] = 0;
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
            battleMusicStrikesSources[currentPhase].clip = musicDataBase.musicsList[chosenMusic].phases[currentPhase].stems[currentStem].stemStrikesAudio;
        }

        for (int i = 0; i < battleMusicPhaseSources.Length; i++)
        {
            battleMusicPhaseSources[i].Play();
            battleMusicPhaseSources[i].time = 0;

            battleMusicStrikesSources[i].Play();
            battleMusicStrikesSources[i].time = 0;
        }
    }

    // Resets music parameters for next match
    public void ResetBattleMusicPhase()
    {
        // Resets phase and stem to 0
        ModifyMusicPhaseToPlayParameters(0, 0, false);

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
