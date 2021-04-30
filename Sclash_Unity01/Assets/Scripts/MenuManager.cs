using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Audio;
using UnityEngine.UI;

using TMPro;











// For Sclash
// NOT OPTIMIZED I THINK

// REQUIREMENTS
// LanguageManager script (Single instance)
// GameManager script (Single instance)
// CharactersDatabse scriptable object
// MenuParameters scriptable object
// TextMeshPro package

/// <summary>
/// Manages menus in the duel scene, but only partly, the code is quite a mess on this side
/// </summary>


// Unity 2019.1.1f1
public class MenuManager : MonoBehaviour
{
    # region VARIABLES
    // SINGLETON    
    public static MenuManager Instance = null;




    [Header("MENU ELEMENTS")]
    [SerializeField] GameObject backIndicator = null;




    [Header("DATA")]
    [SerializeField] CharactersDatabase charactersData = null;



    [Header("SAVE DATA")]
    [SerializeField] public MenuParameters menuParametersSaveScriptableObject = null;




    #region AUDIO SETTINGS
    [Header("AUDIO SETTINGS")]
    [Tooltip("The references to the Sliders components in the options menu controlling the volumes of the different tracks of the game")]
    [SerializeField] SliderToVolume masterVolume = null;
    [SerializeField] SliderToVolume menuMusicVolume = null;
    [SerializeField] SliderToVolume battleMusicVolume = null;
    [SerializeField] SliderToVolume menuFXVolume = null;
    [SerializeField] SliderToVolume fxVolume = null;
    [SerializeField] SliderToVolume voiceVolume = null;
    # endregion




    [Header("GAME SETTINGS")]
    [Tooltip("The reference to the Slider component in the options menu controlling the number of rounds needed to win the game")]
    [SerializeField] Slider roundsToWinSlider = null;
    [SerializeField] GameObject displayHelpCheckBox = null;



    [Header("ERGONOMY SETTINGS")]
    [SerializeField] GameObject rumbleCheckBox = null;
    [SerializeField] TextMeshProUGUI languageText = null;
    [SerializeField] TextMeshProUGUI voicesLanguageText = null;
    [SerializeField] GameObject pingCheckBox = null;





    # region PAUSE
    [Header("PAUSE MENU")]
    [Tooltip("The reference to the menu's blur panel game object")]
    [SerializeField] GameObject blurPanel = null;
    [Tooltip("Menu elements references")]
    [SerializeField] public GameObject pauseMenu = null;
    [SerializeField] public GameObject mainMenu = null;
    [SerializeField] public GameObject winScreen = null;

    int playerWhoPaused = 0;

    //[Tooltip("The duration during which the players can't input the pause again when they already input it")]
    float pauseCooldownDuration = 0.5f;
    float pauseCooldownStartTime = 0f;
    bool pauseCooldownOn = false;
    bool canPauseOn = true;
    # endregion




    # region WIN
    [Header("WIN MENU")]
    [Tooltip("The reference to the game object containing the win text")]
    [SerializeField] public GameObject winMessage = null;
    [Tooltip("The reference to the TextMeshProUGUI component containing the name of the winner")]
    [SerializeField] public TextMeshProUGUI winName = null;
    [SerializeField] public List<TextMeshProUGUI> scoresNames = new List<TextMeshProUGUI>(2);
    [SerializeField] public List<Text> scoresDisplays = new List<Text>(2);
    # endregion
    # endregion
















    # region FUNCTIONS
    # region BASE FUNCTIONS
    private void Awake()                                                                                                         // AWAKE
    {
        Instance = this;
    }


    void Start()                                                                                                                 // START
    {
        // FILLS SCRIPTABLE OBJECTS
        LoadAudioSaveInScriptableObject();
        LoadGameSaveInScriptableObject();
        LoadErgonomySaveInScriptableObject();


        // FILLS SCENE FROM SCRIPTABLE OBJECTS
        SetUpAudioSettingsFromScriptableObject();
        SetUpGameSettingsFromScriptableObject();
        SetUpErgonomySettingsFromScriptableObject();


        Cursor.visible = true;
    }


    void Update()                                                                                                                // UPDATE
    {
        if (enabled && isActiveAndEnabled)
        {
            if (GameManager.Instance)
                switch (GameManager.Instance.gameState)
                {
                    case GameManager.GAMESTATE.game:                        // GAME
                        if (canPauseOn)
                            ManagePauseOnInput();
                        ManageInfosDisplayInput();
                        break;

                    case GameManager.GAMESTATE.paused:                      // PAUSED
                        ManagePauseOutInput();
                        break;
                }


            if (pauseCooldownOn && Time.time - pauseCooldownStartTime > pauseCooldownDuration)
                pauseCooldownOn = false;
        }
    }
    # endregion






    # region INFO DISPLAY IN GAME
    // Update info display
    void ManageInfosDisplayInput()                                                                                                                                      // MANAGE INFO DISPLAY INPUT
    {
        bool playerDead = false;

        for (int i = 0; i < GameManager.Instance.playersList.Count; i++)
        {
            if (GameManager.Instance.playersList[i] != null)
                if (GameManager.Instance.playersList[i].GetComponent<Player>().playerState == Player.STATE.dead)
                    playerDead = true;
        }

        // Display score & in game help
        if (GameManager.Instance)
        {
            if (!playerDead && GameManager.Instance.gameState == GameManager.GAMESTATE.game)
            {
                if (GameManager.Instance.scoreObject)
                    GameManager.Instance.scoreObject.GetComponent<Animator>().SetBool("On", InputManager.Instance.scoreInput);


                for (int i = 0; i < GameManager.Instance.playersList.Count; i++)
                {
                    // ONLINE
                    if (ConnectManager.Instance != null && ConnectManager.Instance.connectedToMaster && i > 0)
                        break;
                    if (GameManager.Instance.inGameHelp.Length > i && InputManager.Instance && InputManager.Instance.playerInputs.Length > i)
                        GameManager.Instance.inGameHelp[i].SetBool("On", InputManager.Instance.playerInputs[i].score);
                    if (GameManager.Instance.playersList[i] != null && InputManager.Instance && InputManager.Instance.playerInputs != null && InputManager.Instance.playerInputs.Length > i)
                        GameManager.Instance.playersList[i].GetComponent<PlayerAnimations>().nameDisplayAnimator.SetBool("On", InputManager.Instance.playerInputs[i].score);
                }
            }
            // Display in game help key indication
            if (!playerDead && GameManager.Instance.gameState == GameManager.GAMESTATE.game)
                for (int i = 0; i < GameManager.Instance.playersList.Count; i++)
                    if (GameManager.Instance.playerKeysIndicators.Count > i && GameManager.Instance.playerKeysIndicators[i] != null && InputManager.Instance.playerInputs.Length > i)
                        GameManager.Instance.playerKeysIndicators[i].SetBool("On", !InputManager.Instance.playerInputs[i].score);
        }
    }
    # endregion






    # region PAUSE
    // PAUSE INPUT
    // Input to activate pause
    void ManagePauseOnInput()                                                                                                                                           // MANAGE PAUSE ON INPUT    
    {
        if (!pauseCooldownOn)
            for (int i = 0; i < InputManager.Instance.playerInputs.Length; i++)
                if (InputManager.Instance.playerInputs[i].pauseUp)
                {
                    playerWhoPaused = i;
                    pauseCooldownOn = true;
                    pauseCooldownStartTime = Time.time;
                    TriggerPause(true);
                }
    }
    // PAUDE OFF INPUT
    // Input to deactivate pause by the player who activated it only
    void ManagePauseOutInput()                                                                                                                                          // MANAGE PAUSE OUT INPUT
    {
        if (!pauseCooldownOn)
            if (InputManager.Instance.playerInputs[playerWhoPaused].pauseUp)
            {
                pauseCooldownOn = false;
                pauseCooldownStartTime = Time.time;
                TriggerPause(false);
            }
    }


    // SWITCH PAUSED
    public void SwitchPause(bool actuallyDoSomething = true)                                                                                                            // SWITCH PAUSE
    {
        if (actuallyDoSomething)
        {
            if (GameManager.Instance.gameState == GameManager.GAMESTATE.paused)
                TriggerPause(false);
            else
                TriggerPause(true);
        }
    }
    // PAUSE OFF
    public void PauseOff()                                                                                                                                              // PAUSE OFF
    {
        TriggerPause(false);
    }
    public void PauseCooldown()                                                                                                                                         // PAUSE COOLDOWN
    {
        pauseCooldownStartTime = Time.time;
        pauseCooldownOn = true;
    }


    // PAUSE FUNCTION
    public void TriggerPause(bool state)                                                                                                                                 // TRIGGER PAUSE
    {
        if (state)
        {
            backIndicator.SetActive(true);

            AudioManager.Instance.SwitchAudioState(AudioManager.AUDIOSTATE.pause);
            GameManager.Instance.SwitchState(GameManager.GAMESTATE.paused);

            if ((ConnectManager.Instance && !ConnectManager.Instance.enableMultiplayer) || !ConnectManager.Instance)
                for (int i = 0; i < GameManager.Instance.playersList.Count; i++)
                    GameManager.Instance.playersList[i].GetComponent<Animator>().enabled = false;
        }
        else
        {
            backIndicator.SetActive(false);

            if (GameManager.Instance.playersList != null && GameManager.Instance.playersList.Count > 0)
                for (int i = 0; i < GameManager.Instance.playersList.Count; i++)
                    if (GameManager.Instance.playersList[i] != null && GameManager.Instance.playersList[i].GetComponent<Player>())
                        GameManager.Instance.playersList[i].GetComponent<Player>().canParry = false;

            canPauseOn = false;
            AudioManager.Instance.SwitchAudioState(AudioManager.AUDIOSTATE.pause);

            if (GameManager.Instance.gameState == GameManager.GAMESTATE.paused)
                GameManager.Instance.SwitchState(GameManager.GAMESTATE.game);


            if (GameManager.Instance.playersList != null && GameManager.Instance.playersList.Count > 0)
                for (int i = 0; i < GameManager.Instance.playersList.Count; i++)
                    if (GameManager.Instance.playersList.Count > i && GameManager.Instance.playersList[i] != null)
                        GameManager.Instance.playersList[i].GetComponent<Animator>().enabled = true;

            Invoke("RestoreCanPauseOn", 0.2f);
        }

        if (blurPanel)
            blurPanel.SetActive(state);
        if (pauseMenu)
            pauseMenu.SetActive(state);

        if (GameManager.Instance && GameManager.Instance.scoreObject)
            GameManager.Instance.scoreObject.GetComponent<Animator>().SetBool("On", state);

        if (GameManager.Instance && GameManager.Instance.playersList != null && GameManager.Instance.playersList.Count > 0)
            for (int i = 0; i < GameManager.Instance.playersList.Count; i++)
            {
                if (GameManager.Instance.inGameHelp != null && GameManager.Instance.inGameHelp.Length > i && GameManager.Instance.inGameHelp[i] != null)
                    GameManager.Instance.inGameHelp[i].SetBool("On", state);

                if (GameManager.Instance.playersList.Count > i && GameManager.Instance.playersList[i] != null)
                    GameManager.Instance.playersList[i].GetComponent<PlayerAnimations>().nameDisplayAnimator.SetBool("On", false);

                if (GameManager.Instance.playerKeysIndicators != null && GameManager.Instance.playerKeysIndicators.Count > i && GameManager.Instance.playerKeysIndicators[i] != null)
                    GameManager.Instance.playerKeysIndicators[i].SetBool("On", !state);
            }


        Cursor.visible = state;



        // CAMPAIGN
        if (NarrationEngine.Instance)
            NarrationEngine.Instance.PauseNarration(state);
    }


    void RestoreCanPauseOn()                                                                                                                                                // RESTORE CAN PAUSE ON
    {
        canPauseOn = true;
    }
    #endregion





    // WIN SCREEN
    public void SetUpWinMenu(string winnerName, Color winnerColor, Vector2 score, Color[] playersColors)                                                                        // SET UP WIN MENU
    {
        if (winName)
        {
            winName.text = winnerName;
            winName.color = winnerColor;
        }

        if (GameManager.Instance && GameManager.Instance.playersList != null && GameManager.Instance.playersList.Count > 0)
            for (int i = 0; i < GameManager.Instance.playersList.Count; i++)
            {
                if (scoresNames != null && scoresNames.Count > i && scoresNames[i] != null)
                {
                    scoresNames[i].text = charactersData.charactersList[GameManager.Instance.playersList[i].GetComponent<Player>().characterIndex].name;
                    scoresNames[i].color = playersColors[i];
                }
                if (scoresDisplays != null && scoresDisplays.Count > i && scoresDisplays[i] != null)
                {
                    scoresDisplays[i].text = score[i].ToString();
                    scoresDisplays[i].color = playersColors[i];
                }
            }
    }







    #region SAVE / LOAD PARAMETERS
    // GAME SETTINGS FROM SAVE TO SCRIPTABLE OBJECTS
    public void LoadGameSaveInScriptableObject()                                                                                                                         // LOAD GAME SAVE IN SCRIPTABLE OBJECT
    {
        JsonSave save = SaveGameManager.GetCurrentSave();

        if (menuParametersSaveScriptableObject)
        {
            menuParametersSaveScriptableObject.displayHelp = save.displayHelp;
            menuParametersSaveScriptableObject.roundToWin = save.roundsToWin;
        }
    }
    // ERGONOMY SETTINGS FROM SAVE TO SCRIPTABLE OBJECTS
    public void LoadErgonomySaveInScriptableObject()                                                                                                         // LOAD ERGONOMY SAVE IN SCRIPTABLE OBJECT
    {
        JsonSave save = SaveGameManager.GetCurrentSave();

        if (menuParametersSaveScriptableObject)
        {
            menuParametersSaveScriptableObject.enableRumbles = save.enableRumbles;
            menuParametersSaveScriptableObject.language = save.language;
            menuParametersSaveScriptableObject.voicesLanguage = save.voicesLanguage;
            menuParametersSaveScriptableObject.displayPing = save.displayPing;
        }
    }
    // AUDIO SETTINGS FROM SAVE TO SCRIPTABLE OBJECT
    public void LoadAudioSaveInScriptableObject()                                                                                                           // LOAD AUDIO SAVE IN SCRIPTABLE OBJECT
    {
        JsonSave save = SaveGameManager.GetCurrentSave();

        if (menuParametersSaveScriptableObject)
        {
            // Master volume
            if (save.masterVolume > menuParametersSaveScriptableObject.maxMasterVolume)
                menuParametersSaveScriptableObject.masterVolume = menuParametersSaveScriptableObject.defaultMasterVolume;
            else
                menuParametersSaveScriptableObject.masterVolume = save.masterVolume;


            // Menu music volume
            if (save.menuMusicVolume > menuParametersSaveScriptableObject.maxMenuMusicVolume)
                menuParametersSaveScriptableObject.menuMusicVolume = menuParametersSaveScriptableObject.defaultMenuMusicVolume;
            else
                menuParametersSaveScriptableObject.menuMusicVolume = save.menuMusicVolume;


            // Battle music volume
            if (save.battleMusicVolume > menuParametersSaveScriptableObject.maxBattleMusicVolume)
                menuParametersSaveScriptableObject.battleMusicVolume = menuParametersSaveScriptableObject.defaultBattleMusicVolume;
            else
                menuParametersSaveScriptableObject.battleMusicVolume = save.battleMusicVolume;


            // Menu FX volume
            if (save.menuFXVolume > menuParametersSaveScriptableObject.maxMenuFXVolume)
                menuParametersSaveScriptableObject.menuFXVolume = menuParametersSaveScriptableObject.defaultMenuFXVolume;
            else
                menuParametersSaveScriptableObject.menuFXVolume = save.menuFXVolume;


            // FX Volume
            if (save.fxVolume > menuParametersSaveScriptableObject.maxFxVolume)
                menuParametersSaveScriptableObject.fxVolume = menuParametersSaveScriptableObject.defaultFxVolume;
            else
                menuParametersSaveScriptableObject.fxVolume = save.fxVolume;


            // Voices volume
            if (save.voiceVolume > menuParametersSaveScriptableObject.maxVoiceVolume)
                menuParametersSaveScriptableObject.voiceVolume = menuParametersSaveScriptableObject.defaultVoiceVolume;
            else
                menuParametersSaveScriptableObject.voiceVolume = save.voiceVolume;
        }
    }


    // SET UP GAME SETTINGS IN SCENE
    public void SetUpGameSettingsFromScriptableObject()                                                                                                          // SET UP GAME SETTINGS FROM SCRIPTABLE OBJECT
    {
        if (menuParametersSaveScriptableObject)
        {
            if (roundsToWinSlider != null)
                roundsToWinSlider.value = menuParametersSaveScriptableObject.roundToWin;
            if (displayHelpCheckBox != null)
                displayHelpCheckBox.SetActive(menuParametersSaveScriptableObject.displayHelp);
            if (rumbleCheckBox != null)
                rumbleCheckBox.SetActive(menuParametersSaveScriptableObject.enableRumbles);
            if (pingCheckBox != null)
                pingCheckBox.SetActive(menuParametersSaveScriptableObject.displayPing);


            if (GameManager.Instance && GameManager.Instance.inGameHelp != null && GameManager.Instance.inGameHelp.Length > 0)
                for (int i = 0; i < GameManager.Instance.inGameHelp.Length; i++)
                {
                    if (GameManager.Instance.inGameHelp[i] != null)
                        GameManager.Instance.inGameHelp[i].gameObject.SetActive(menuParametersSaveScriptableObject.displayHelp);
                    if (GameManager.Instance.playerKeysIndicators != null && GameManager.Instance.playerKeysIndicators.Count > i && GameManager.Instance.playerKeysIndicators[i] != null)
                        GameManager.Instance.playerKeysIndicators[i].gameObject.SetActive(menuParametersSaveScriptableObject.displayHelp);
                    if (GameManager.Instance.drawTextAnimator)
                        GameManager.Instance.drawTextAnimator.gameObject.SetActive(menuParametersSaveScriptableObject.displayHelp);
                }
        }
    }
    // SET UP ERGONOMY SETTINGS IN SCENE
    public void SetUpErgonomySettingsFromScriptableObject()                                                                                                // SET UP ERGONOMY SETTINGS FROM SCRIPTABLE OBJECT
    {
        if (menuParametersSaveScriptableObject)
        {
            if (rumbleCheckBox != null)
                rumbleCheckBox.SetActive(menuParametersSaveScriptableObject.enableRumbles);
            if (pingCheckBox != null)
                pingCheckBox.SetActive(menuParametersSaveScriptableObject.displayPing);
            if (LanguageManager.Instance)
            {
                LanguageManager.Instance.language = menuParametersSaveScriptableObject.language;
                LanguageManager.Instance.voicesLanguage = menuParametersSaveScriptableObject.voicesLanguage;
            }
            if (languageText != null)
                languageText.text = menuParametersSaveScriptableObject.language;
            if (voicesLanguageText != null)
                voicesLanguageText.text = menuParametersSaveScriptableObject.voicesLanguage;
        }
    }
    // SET UP AUDIO SETTINGS
    public void SetUpAudioSettingsFromScriptableObject()                                                                                                // SET UP AUDIO SETTINGS FROM SCRIPTABLE OBJECT
    {
        masterVolume.slider.value = menuParametersSaveScriptableObject.masterVolume;
        masterVolume.UpdateVolume();

        menuMusicVolume.slider.value = menuParametersSaveScriptableObject.menuMusicVolume;
        menuMusicVolume.UpdateVolume();

        battleMusicVolume.slider.value = menuParametersSaveScriptableObject.battleMusicVolume;
        battleMusicVolume.UpdateVolume();

        menuFXVolume.slider.value = menuParametersSaveScriptableObject.menuFXVolume;
        menuFXVolume.UpdateVolume();

        fxVolume.slider.value = menuParametersSaveScriptableObject.fxVolume;
        fxVolume.UpdateVolume();

        voiceVolume.slider.value = menuParametersSaveScriptableObject.voiceVolume;
        voiceVolume.UpdateVolume();
    }


    // SAVE GAME SETTINGS
    public void SaveGameSettingsInScriptableObject()                                                                                                                // SAVE GAME SETTINGS IN SCRIPTABLE OBJECT
    {
        menuParametersSaveScriptableObject.roundToWin = Mathf.FloorToInt(roundsToWinSlider.value);
        menuParametersSaveScriptableObject.displayHelp = displayHelpCheckBox.activeSelf;
        menuParametersSaveScriptableObject.roundToWin = GameManager.Instance.scoreToWin;
    }
    // SAVE GAME SETTINGS
    public void SaveErgonomySettingsInScriptableObject()                                                                                                // SAVE ERGONOMY SETTINGS IN SCRIPTABLE OBJECT
    {
        if (rumbleCheckBox != null)
            menuParametersSaveScriptableObject.enableRumbles = rumbleCheckBox.activeInHierarchy;
        if (pingCheckBox != null)
            menuParametersSaveScriptableObject.displayPing = pingCheckBox.activeInHierarchy;
        if (languageText != null)
            menuParametersSaveScriptableObject.language = languageText.text;
        if (voicesLanguageText != null)
            menuParametersSaveScriptableObject.voicesLanguage = voicesLanguageText.text;
    }
    // SAVE AUDIO SETTINGS
    public void SaveAudioSettingsInScriptableObject()                                                                                               // SAVE AUDIO SETTINGS IN SCRIPTABLE OBJECT
    {
        menuParametersSaveScriptableObject.masterVolume = masterVolume.slider.value;
        menuParametersSaveScriptableObject.menuMusicVolume = menuMusicVolume.slider.value;
        menuParametersSaveScriptableObject.battleMusicVolume = battleMusicVolume.slider.value;
        menuParametersSaveScriptableObject.menuFXVolume = menuFXVolume.slider.value;
        menuParametersSaveScriptableObject.fxVolume = fxVolume.slider.value;
        menuParametersSaveScriptableObject.voiceVolume = voiceVolume.slider.value;
    }


    // GAME SETTINGS FROM SCRIPTABLE OBJECT TO SAVE
    public void SaveGameSettingsFromScriptableObject()                                                                                          // SAVE GAME SETTINGS FROM SCRIPTABLE OBJECT
    {
        JsonSave save = SaveGameManager.GetCurrentSave();

        save.roundsToWin = menuParametersSaveScriptableObject.roundToWin;
        save.displayHelp = menuParametersSaveScriptableObject.displayHelp;

        SaveGameManager.Save();
    }
    // ERGONOMY SETTINGS FROM SCRIPTABLE OBJECT TO SAVE
    public void SaveErgonomySettingsFromScriptableObject()                                                                                  // SAVE ERGONOMY SETTINGS FROM SCRIPTABLE OBJECT
    {
        JsonSave save = SaveGameManager.GetCurrentSave();

        save.enableRumbles = menuParametersSaveScriptableObject.enableRumbles;
        save.language = menuParametersSaveScriptableObject.language;
        save.voicesLanguage = menuParametersSaveScriptableObject.voicesLanguage;
        save.displayPing = menuParametersSaveScriptableObject.displayPing;

        SaveGameManager.Save();
    }
    // AUDIO SETTINGS FROM SCRIPTABLE OBJECT TO SAVE
    public void SaveAudioSettingsFromScriptableObject()                                                                                     // SAVE AUDIO SETTINGS FROM SCRIPTABLE OBJECT
    {
        JsonSave save = SaveGameManager.GetCurrentSave();


        save.masterVolume = menuParametersSaveScriptableObject.masterVolume;
        save.menuMusicVolume = menuParametersSaveScriptableObject.menuMusicVolume;
        save.battleMusicVolume = menuParametersSaveScriptableObject.battleMusicVolume;
        save.menuFXVolume = menuParametersSaveScriptableObject.menuFXVolume;
        save.fxVolume = menuParametersSaveScriptableObject.fxVolume;
        save.voiceVolume = menuParametersSaveScriptableObject.voiceVolume;


        SaveGameManager.Save();
    }
    #endregion
    #endregion
}
