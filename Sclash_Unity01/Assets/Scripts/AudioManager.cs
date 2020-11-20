using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// Script that manages most audio stuff of the game, especially music
// OPTIMIZED I THINK ?
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    #region MANAGERS
    [Header("MANAGERS")]
    // Name of the GameManager to find it in the scene
    [SerializeField] GameManager gameManager = null;
    // Name of the CameraManager to find it in the scene
    [SerializeField] CameraManager cameraManager = null;
    #endregion




    // This one might be useless
    [Header("SOUND FUNCTIONS")]
    // A script that provides premade sound functions to play a little more easily with audio sources
    [SerializeField] SoundFunctions soundFunctions = null;



    [Header("MUSIC DATA")]
    [SerializeField] MusicsDatabase musicDataBase = null;



    #region AUDIO STATES
    public enum AUDIOSTATE
    {
        none,
        menu,
        beforeBattle,
        battle,
        pause,
        won,
    }

    [Header("AUDIO STATES")]
    public AUDIOSTATE audioState = AUDIOSTATE.none;
    AUDIOSTATE oldAudioState = AUDIOSTATE.none;
    #endregion




    #region MUSIC SOURCES
    [Header("MUSIC SOURCES")]
    [SerializeField] AudioSource menuAudioSource = null;
    [SerializeField] public List<AudioSource> phasesMainAudioSources = new List<AudioSource>(3) { null, null, null };
    [SerializeField] public List<AudioSource> phasesStrikesAudioSources = new List<AudioSource>(3) { null, null, null };
    [SerializeField] public AudioSource winMusicAudioSource = null;
    [SerializeField] AudioSource phaseTransitionFXAudioSource = null;
    [SerializeField] AudioSource phaseTransitionStemAudioSource = null;
    [SerializeField] AudioSource windAudioSource = null;
    #endregion



    #region MUSIC SOURCES VOLUMES PARAMETERS
    // SOURCES VOLUMES OBJECTIVES (The volumes towards which the sources' volumes will move slowly)
    [Header("MUSIC SOURCES VOLUMES PARAMETERS")]
    float menuVolumeObjective = 0;
    List<float> phasesMainVolumeObjectives = new List<float>(3) { 0, 0, 0 };
    List<float> phasesStrikesVolumeObjectives = new List<float>(3) { 0, 0, 0 };
    [SerializeField] Vector2 playersDistanceForStrikesVolumeLimits = new Vector2(6, 15);
    //float winVolumeObjective = 0;
    float windVolumeObjective = 0;
    [SerializeField] float volumeFadeSpeed = 0.01f;
    #endregion



    #region SOURCES MAX VOLUMES
    float maxMenuVolume = 0;
    List<float> maxPhasesMainVolumes = new List<float>(3) { 0, 0, 0 };
    List<float> maxPhasesStrikesVolumes = new List<float>(3) { 0, 0, 0 };
    float maxWinVolume = 0;
    float maxWindVolume = 0;
    #endregion



    #region SELECTED MUSIC, STEM, PHASE
    [Header("STEMS & PHASES")]
    [SerializeField] bool useRandomStemSelection = true;
    [SerializeField]
    int maxPhase = 2,
        scoreFromMaxScoreToAutoMaxPhase = 1;
    bool decrementCurrentPhaseAtNextLoop = false;
    [HideInInspector] public int currentlySelectedMusicIndex = 0;
    [SerializeField]
    int currentMusicPhase = 0,
        currentMusicStem = 0,
        previousMusicStem = 0;
    #endregion



    #region BATTLE INTENSITY
    [Header("MUSIC BATTLE INTENSITY")]
    [SerializeField] float decreaseBattleIntensityEveryDuration = 3f;
    float decreaseBattleIntensityLoopStartTime = 0f;
    int currentBattleIntensity = 0;
    int lastBattleIntensityLevel = 0;
    [SerializeField] int maxBattleIntensityLevel = 12;
    [SerializeField] List<int> battleIntensityLevelsForPhaseUp = new List<int>(2) { 3, 8, 1000 };
    #endregion




    #region SOUND FX
    [Header("SOUND FX")]
    [SerializeField] AudioSource parrySoundFXAudioSource = null;
    [SerializeField] AudioSource successfulAttackSoundFXAudioSource = null;
    [SerializeField] AudioSource deathSoundFXAudioSource = null;
    [SerializeField] AudioSource slowMoInAudioSource = null;
    [SerializeField] AudioSource slowMoOutAudioSource = null;
    // Sound FXs audio sources but with a script to play a random sound in a previously filled list
    [SerializeField] public PlayRandomSoundInList matchBeginsRandomSoundSource = null;
    [SerializeField] public PlayRandomSoundInList roundBeginsRandomSoundSource = null;
    #endregion





    #region CLASH FX
    [Header("CLASH FX")]
    [SerializeField] AudioSource clashImpactSoundFXAudioSOurce = null;
    [SerializeField] AudioSource clashReverbSoundFXAudioSource = null;
    [SerializeField] float clashReverbDelay = 0.3f;
    #endregion




    GameObject[] playersList;
    //float distanceBetweenPlayers = 0;




    //[Header("CHEATS")]
    //[SerializeField] KeyCode phaseUpCheatKey = KeyCode.Alpha8;
    //[SerializeField] KeyCode phaseDownCheatKey = KeyCode.Alpha7;















    #region FUNCTIONS
    #region BASE FUNCTIONS
    private void Awake()
    {
        Instance = this;
        GetSourcesDefaultVolume();
    }


    // Start is called before the first frame update
    void Start()
    {
        // Finds the players in the scene to use their data
        FindPlayers();
    }


    // FixedUpdate is called 50 times per frame
    private void FixedUpdate()
    {
        if (enabled && isActiveAndEnabled)
            switch (audioState)
            {
                case AUDIOSTATE.none:                                       // NONE
                    RunSmoothVolumesUpdates();
                    break;


                case AUDIOSTATE.menu:                                           // MENU
                    RunSmoothVolumesUpdates();
                    break;


                case AUDIOSTATE.beforeBattle:                                             // BEFORE BATTLE
                    RunSmoothVolumesUpdates();
                    break;


                case AUDIOSTATE.battle:                                                 // BATTLE
                    RunSmoothVolumesUpdates();
                    FadeStrikesVolumeObjectiveDependingOnPlayersDistance();
                    DetectStemEndAndStartNextOne();
                    DecreaseIntensityWithTime();
                    break;


                case AUDIOSTATE.pause:                                                      // PAUSE
                    RunSmoothVolumesUpdates();
                    FadeStrikesVolumeObjectiveDependingOnPlayersDistance();
                    DetectStemEndAndStartNextOne();
                    break;


                case AUDIOSTATE.won:                                                        // WON
                    RunSmoothVolumesUpdates();
                    break;
            }
    }
    #endregion




    #region AUDIO STATES
    public void SwitchAudioState(AUDIOSTATE newAudioState)
    {
        oldAudioState = audioState;
        audioState = newAudioState;


        switch (newAudioState)
        {
            case AUDIOSTATE.none:                                                       // NONE
                for (int i = 0; i < phasesMainVolumeObjectives.Count; i++)
                    phasesMainVolumeObjectives[i] = 0;
                for (int i = 0; i < phasesStrikesVolumeObjectives.Count; i++)
                    phasesStrikesVolumeObjectives[i] = 0;
                break;


            case AUDIOSTATE.menu:                                                   // MENU
                windVolumeObjective = 0;
                menuVolumeObjective = maxMenuVolume;
                menuAudioSource.Play();
                break;


            case AUDIOSTATE.beforeBattle:                                           // BEFORE BATTLE
                menuVolumeObjective = 0;
                windAudioSource.Play();
                windVolumeObjective = maxWindVolume;
                SetUpAudioElementsDependingOnSelectedMusic(currentlySelectedMusicIndex);
                phasesMainVolumeObjectives[currentMusicPhase] = maxPhasesMainVolumes[currentMusicPhase];

                for (int i = 0; i < phasesMainAudioSources.Count; i++)
                    phasesMainAudioSources[i].Stop();
                for (int i = 0; i < phasesStrikesAudioSources.Count; i++)
                    phasesStrikesAudioSources[i].Stop();
                break;


            case AUDIOSTATE.battle:                                                 // BATTLE
                windVolumeObjective = 0;
                phasesMainAudioSources[currentMusicPhase].Play();
                phasesStrikesAudioSources[currentMusicPhase].Play();


                for (int i = 0; i < phasesMainAudioSources.Count; i++)
                    phasesMainAudioSources[i].Play();
                for (int i = 0; i < phasesStrikesAudioSources.Count; i++)
                    phasesStrikesAudioSources[i].Play();
                break;


            case AUDIOSTATE.pause:
                break;


            case AUDIOSTATE.won:                                                    // WON
                windVolumeObjective = maxWindVolume;
                for (int i = 0; i < phasesMainVolumeObjectives.Count; i++)
                    phasesMainVolumeObjectives[i] = 0;
                for (int i = 0; i < phasesStrikesVolumeObjectives.Count; i++)
                    phasesStrikesVolumeObjectives[i] = 0;

                for (int i = 0; i < phasesMainAudioSources.Count; i++)
                    phasesMainAudioSources[i].Stop();
                for (int i = 0; i < phasesStrikesAudioSources.Count; i++)
                    phasesStrikesAudioSources[i].Stop();
                break;
        }
    }
    #endregion








    #region SET UP MUSIC
    public void ChangeSelectedMusicIndex(int newIndex)
    {
        if (newIndex < musicDataBase.musicsList.Count)
            currentlySelectedMusicIndex = newIndex;
    }


    void SetUpAudioElementsDependingOnSelectedMusic(int selectedMusic)
    {
        for (int i = 0; i < 3; i++)
        {
            phasesMainAudioSources[i].clip = musicDataBase.musicsList[currentlySelectedMusicIndex].phases[i].stems[musicDataBase.musicsList[currentlySelectedMusicIndex].startStem].stemAudio;
            phasesMainAudioSources[i].clip = musicDataBase.musicsList[currentlySelectedMusicIndex].phases[i].stems[musicDataBase.musicsList[currentlySelectedMusicIndex].startStem].stemAudio;
            phasesMainAudioSources[i].clip = musicDataBase.musicsList[currentlySelectedMusicIndex].phases[i].stems[musicDataBase.musicsList[currentlySelectedMusicIndex].startStem].stemAudio;

            phasesStrikesAudioSources[i].clip = musicDataBase.musicsList[currentlySelectedMusicIndex].phases[i].stems[musicDataBase.musicsList[currentlySelectedMusicIndex].startStem].stemStrikesAudio;
            phasesStrikesAudioSources[i].clip = musicDataBase.musicsList[currentlySelectedMusicIndex].phases[i].stems[musicDataBase.musicsList[currentlySelectedMusicIndex].startStem].stemStrikesAudio;
            phasesStrikesAudioSources[i].clip = musicDataBase.musicsList[currentlySelectedMusicIndex].phases[i].stems[musicDataBase.musicsList[currentlySelectedMusicIndex].startStem].stemStrikesAudio;
        }


        ImmediatelySwitchPhase(0, true, 0);
        currentMusicStem = musicDataBase.musicsList[currentlySelectedMusicIndex].startStem;
        previousMusicStem = musicDataBase.musicsList[currentlySelectedMusicIndex].startStem;
        currentBattleIntensity = 0;
        lastBattleIntensityLevel = 0;


        winMusicAudioSource.clip = musicDataBase.musicsList[currentlySelectedMusicIndex].winAudio;
    }
    #endregion




    #region MUSIC INTENSITY / EVENT
    public void BattleEventIncreaseIntensity()
    {
        lastBattleIntensityLevel = currentBattleIntensity;
        currentBattleIntensity++;


        if (currentBattleIntensity >= maxBattleIntensityLevel)
            currentBattleIntensity = maxBattleIntensityLevel;


        UpdateMusicDependingOnBattleIntensity();
        decreaseBattleIntensityLoopStartTime = Time.time;
    }


    void DecreaseIntensityWithTime()
    {
        int currentMaxScore = Mathf.FloorToInt(Mathf.Max(gameManager.score[0], gameManager.score[1]));

        if (currentBattleIntensity > 0 && (currentMaxScore < gameManager.scoreToWin - scoreFromMaxScoreToAutoMaxPhase))
            if (Time.time - decreaseBattleIntensityLoopStartTime >= decreaseBattleIntensityEveryDuration)
            {
                lastBattleIntensityLevel = currentBattleIntensity;
                currentBattleIntensity--;

                UpdateMusicDependingOnBattleIntensity();
                decreaseBattleIntensityLoopStartTime = Time.time;
            }
    }


    void UpdateMusicDependingOnBattleIntensity()
    {
        int currentMaxScore = Mathf.FloorToInt(Mathf.Max(gameManager.score[0], gameManager.score[1]));


        if (!(currentMaxScore >= gameManager.scoreToWin - scoreFromMaxScoreToAutoMaxPhase && gameManager.scoreToWin > scoreFromMaxScoreToAutoMaxPhase))
        {
            if (currentMusicPhase < phasesMainAudioSources.Count - 1)
            {
                if (currentBattleIntensity >= battleIntensityLevelsForPhaseUp[currentMusicPhase] && lastBattleIntensityLevel < battleIntensityLevelsForPhaseUp[currentMusicPhase])
                {
                    ImmediatelySwitchPhase(1, false, 0);
                    phaseTransitionFXAudioSource.clip = musicDataBase.musicsList[currentlySelectedMusicIndex].phases[currentMusicPhase].phaseUpFX;
                    phaseTransitionFXAudioSource.Play();
                }
            }
            if (currentMusicPhase > 0)
            {
                if (currentBattleIntensity < battleIntensityLevelsForPhaseUp[currentMusicPhase - 1] && lastBattleIntensityLevel >= battleIntensityLevelsForPhaseUp[currentMusicPhase - 1])
                    decrementCurrentPhaseAtNextLoop = true;
                if (decrementCurrentPhaseAtNextLoop && currentBattleIntensity >= battleIntensityLevelsForPhaseUp[currentMusicPhase - 1] && lastBattleIntensityLevel < battleIntensityLevelsForPhaseUp[currentMusicPhase - 1])
                    decrementCurrentPhaseAtNextLoop = false;
            }
        }
    }
    #endregion




    #region CONTINUE MUSIC LOOP
    void DetectStemEndAndStartNextOne()
    {
        if (phasesMainAudioSources[currentMusicPhase].time >= 11.98f)
        {
            bool isMaxPhase = false;
            SwitchStem(useRandomStemSelection);
            int currentMaxScore = Mathf.FloorToInt(Mathf.Max(gameManager.score[0], gameManager.score[1]));


            if (currentMaxScore >= gameManager.scoreToWin - scoreFromMaxScoreToAutoMaxPhase & gameManager.scoreToWin > scoreFromMaxScoreToAutoMaxPhase)
            {
                if (currentMusicPhase < maxPhase)
                    phaseTransitionStemAudioSource.Play();

                isMaxPhase = true;
                ImmediatelySwitchPhase(0, true, maxPhase);
            }
            // Adds phase down transition stem on top of normal stem
            else if (decrementCurrentPhaseAtNextLoop)
            {
                ImmediatelySwitchPhase(-1, false, 0);

                if (musicDataBase.musicsList[currentlySelectedMusicIndex].phases[currentMusicPhase].phaseDownStems.Count > 0)
                    phaseTransitionStemAudioSource.clip = musicDataBase.musicsList[currentlySelectedMusicIndex].phases[currentMusicPhase].phaseDownStems[Random.Range(0, musicDataBase.musicsList[currentlySelectedMusicIndex].phases[currentMusicPhase].phaseDownStems.Count - 1)].stemAudio;


                phaseTransitionStemAudioSource.Play();
            }


            for (int i = 0; i < phasesMainAudioSources.Count; i++)
            {
                phasesMainAudioSources[i].clip = musicDataBase.musicsList[currentlySelectedMusicIndex].phases[i].stems[currentMusicStem].stemAudio;
                phasesMainAudioSources[i].Play();
            }
            for (int i = 0; i < phasesStrikesAudioSources.Count; i++)
            {
                // Sets transitions stem strikes if there are for this stem
                if (!isMaxPhase && decrementCurrentPhaseAtNextLoop && musicDataBase.musicsList[currentlySelectedMusicIndex].phases[i].phaseDownStems.Count > 0)
                    phasesStrikesAudioSources[i].clip = musicDataBase.musicsList[currentlySelectedMusicIndex].phases[i].phaseDownStems[Random.Range(0, musicDataBase.musicsList[currentlySelectedMusicIndex].phases[currentMusicPhase].phaseDownStems.Count - 1)].stemStrikesAudio;
                // Sets next stem strikes if there are for this stem
                else if (musicDataBase.musicsList[currentlySelectedMusicIndex].phases[i].stems[currentMusicStem].stemStrikesAudio)
                    phasesStrikesAudioSources[i].clip = musicDataBase.musicsList[currentlySelectedMusicIndex].phases[i].stems[currentMusicStem].stemStrikesAudio;


                phasesStrikesAudioSources[i].Play();
            }

            decrementCurrentPhaseAtNextLoop = false;
        }
    }
    #endregion




    #region CHANGE STEM / PHASE
    void ImmediatelySwitchPhase(int incrementation, bool absolutePhase, int newPhase)
    {
        if (absolutePhase)
            currentMusicPhase = newPhase;
        else
            currentMusicPhase += incrementation;


        if (currentMusicPhase <= 0)
            currentMusicPhase = 0;
        else if (currentMusicPhase >= phasesMainAudioSources.Count - 1)
            currentMusicPhase = phasesMainAudioSources.Count - 1;


        for (int i = 0; i < phasesMainVolumeObjectives.Count; i++)
        {
            if (i == currentMusicPhase)
                phasesMainVolumeObjectives[i] = maxPhasesMainVolumes[i];
            else
                phasesMainVolumeObjectives[i] = 0;
        }
        for (int i = 0; i < phasesStrikesVolumeObjectives.Count; i++)
        {
            if (i == currentMusicPhase)
                phasesStrikesVolumeObjectives[i] = maxPhasesStrikesVolumes[i];
            else
                phasesStrikesVolumeObjectives[i] = 0;
        }
    }


    void SwitchStem(bool random)
    {
        previousMusicStem = currentMusicStem;


        if (!random)
        {
            currentMusicStem++;

            if (currentMusicStem >= musicDataBase.musicsList[currentlySelectedMusicIndex].phases[currentMusicPhase].stems.Count)
                currentMusicStem = 0;
        }
        else
        {
            int oldStem = currentMusicStem;

            while (oldStem == currentMusicStem || previousMusicStem == currentMusicStem)
                currentMusicStem = Random.Range(0, musicDataBase.musicsList[currentlySelectedMusicIndex].phases[currentMusicPhase].stems.Count - 1);
        }
    }
    #endregion




    #region MUSICS VOLUMES
    void GetSourcesDefaultVolume()
    {
        maxMenuVolume = menuAudioSource.volume;
        maxWinVolume = winMusicAudioSource.volume;
        maxWindVolume = windAudioSource.volume;


        for (int i = 0; i < maxPhasesMainVolumes.Count; i++)
            maxPhasesMainVolumes[i] = phasesMainAudioSources[i].volume;
        for (int i = 0; i < maxPhasesStrikesVolumes.Count; i++)
            maxPhasesStrikesVolumes[i] = phasesStrikesAudioSources[i].volume;
    }


    void RunSmoothVolumesUpdates()
    {
        float volumeFadeDirection = 0;


        // WIND
        if (windAudioSource.volume != windVolumeObjective)
        {
            volumeFadeDirection = Mathf.Sign(windVolumeObjective - windAudioSource.volume);

            windAudioSource.volume += volumeFadeDirection * volumeFadeSpeed;

            if (volumeFadeDirection >= 0 && windAudioSource.volume >= windVolumeObjective)
                windAudioSource.volume = windVolumeObjective;
            else if (volumeFadeDirection < 0 && windAudioSource.volume <= windVolumeObjective)
                windAudioSource.volume = windVolumeObjective;
        }

        // MENU
        if (menuAudioSource.volume != menuVolumeObjective)
        {
            volumeFadeDirection = Mathf.Sign(menuVolumeObjective - menuAudioSource.volume);

            menuAudioSource.volume += volumeFadeDirection * volumeFadeSpeed;

            if (volumeFadeDirection >= 0 && menuAudioSource.volume >= menuVolumeObjective)
                menuAudioSource.volume = menuVolumeObjective;
            else if (volumeFadeDirection < 0 && menuAudioSource.volume <= menuVolumeObjective)
                menuAudioSource.volume = menuVolumeObjective;
        }

        // MAIN
        for (int i = 0; i < phasesMainVolumeObjectives.Count; i++)
            if (phasesMainAudioSources[i].volume != phasesMainVolumeObjectives[i])
            {
                volumeFadeDirection = Mathf.Sign(phasesMainVolumeObjectives[i] - phasesMainAudioSources[i].volume);

                phasesMainAudioSources[i].volume += volumeFadeDirection * volumeFadeSpeed;

                if (volumeFadeDirection >= 0 && phasesMainAudioSources[i].volume >= phasesMainVolumeObjectives[i])
                    phasesMainAudioSources[i].volume = phasesMainVolumeObjectives[i];
                else if (volumeFadeDirection < 0 && phasesMainAudioSources[i].volume <= phasesMainVolumeObjectives[i])
                    phasesMainAudioSources[i].volume = phasesMainVolumeObjectives[i];
            }

        // STRIKES
        for (int i = 0; i < phasesStrikesVolumeObjectives.Count; i++)
            if (phasesStrikesAudioSources[i].volume != phasesStrikesVolumeObjectives[i])
            {
                volumeFadeDirection = Mathf.Sign(phasesStrikesVolumeObjectives[i] - phasesStrikesAudioSources[i].volume);

                phasesStrikesAudioSources[i].volume += volumeFadeDirection * volumeFadeSpeed;

                if (volumeFadeDirection >= 0 && phasesStrikesAudioSources[i].volume >= phasesStrikesVolumeObjectives[i])
                    phasesStrikesAudioSources[i].volume = phasesStrikesVolumeObjectives[i];
                else if (volumeFadeDirection < 0 && phasesStrikesAudioSources[i].volume <= phasesStrikesVolumeObjectives[i])
                    phasesStrikesAudioSources[i].volume = phasesStrikesVolumeObjectives[i];
            }
    }

    void FadeStrikesVolumeObjectiveDependingOnPlayersDistance()
    {
        float tempVolume = ((cameraManager.actualDistanceBetweenPlayers - playersDistanceForStrikesVolumeLimits.y) / (playersDistanceForStrikesVolumeLimits.x - playersDistanceForStrikesVolumeLimits.y)) * maxPhasesStrikesVolumes[currentMusicPhase];


        if (tempVolume < 0)
            tempVolume = 0;
        else if (tempVolume > maxPhasesStrikesVolumes[currentMusicPhase])
            tempVolume = maxPhasesStrikesVolumes[currentMusicPhase];

        phasesStrikesVolumeObjectives[currentMusicPhase] = tempVolume;
    }
    #endregion





    #region SOUND FX
    // Plays clash sound FX
    public void TriggerClashAudioCoroutine()
    {
        StartCoroutine(ClashAudioCoroutine());
    }


    IEnumerator ClashAudioCoroutine()
    {
        soundFunctions.PlaySoundFromSource(clashImpactSoundFXAudioSOurce);
        yield return new WaitForSecondsRealtime(clashReverbDelay);
        soundFunctions.PlaySoundFromSource(clashReverbSoundFXAudioSource);
    }


    // Plays successful parry sound FX
    public void TriggerParriedAudio()
    {
        soundFunctions.PlaySoundFromSource(parrySoundFXAudioSource);
    }


    // Triggers successful attack sound FX play
    public void TriggerSuccessfulAttackAudio()
    {
        soundFunctions.PlaySoundFromSource(successfulAttackSoundFXAudioSource);
        soundFunctions.PlaySoundFromSource(deathSoundFXAudioSource);
    }


    public void TriggerSlowMoAudio(bool inOut)
    {
        if (inOut)
            soundFunctions.PlaySoundFromSource(slowMoInAudioSource);
        else
            soundFunctions.PlaySoundFromSource(slowMoOutAudioSource);
    }
    #endregion





    // FIND PLAYERS
    // Finds the players on the scene to use their data for music modification
    public void FindPlayers()
    {
        playersList = GameObject.FindGameObjectsWithTag("Player");
    }
    #endregion
}