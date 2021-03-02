using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;


// Manages menus in the duel scene, but only partly, the code is quite a mess on this side
// Created for Unity 2019.1.1f1
// NNOT OPTIMIZED
public class MenuManager : MonoBehaviour
{
    # region VARIABLES
    // SINGLETON    
    public static MenuManager Instance = null;


    # region MANAGERS
    [Header("MANAGERS")]
    [Tooltip("The references to the GameManager script instance component")]
    [SerializeField] GameManager gameManager = null;

    [Tooltip("The AudioManager script instance reference")]
    [SerializeField] AudioManager audioManager = null;

    [Tooltip("The references to the InputManager script instance component")]
    [SerializeField] InputManager inputManager = null;
    #endregion



    #region MENU ELEMENTS
    [SerializeField] GameObject backIndicator = null;
    #endregion





    #region DATA
    [Header("DATA")]
    [SerializeField] CharactersDatabase charactersData = null;
    #endregion




    # region SAVE DATA
    [Header("SAVE DATA")]
    [SerializeField] MenuParameters menuParametersSaveScriptableObject = null;
    # endregion




    #region AUDIO SETTINGS
    [Header("AUDIO SETTINGS")]
    [Tooltip("The references to the Sliders components in the options menu controlling the volumes of the different tracks of the game")]
    [SerializeField]
    SliderToVolume masterVolume = null;
    [SerializeField]
    SliderToVolume menuMusicVolume = null,
        battleMusicVolume = null,
        menuFXVolume = null,
        fxVolume = null,
        voiceVolume = null;
    # endregion




    #region GAME SETTINGS
    [Header("GAME SETTINGS")]
    [Tooltip("The reference to the Slider component in the options menu controlling the number of rounds needed to win the game")]
    [SerializeField] Slider roundsToWinSlider = null;
    [SerializeField] GameObject displayHelpCheckBox = null;
    # endregion





    # region PAUSE
    [Header("PAUSE MENU")]
    [Tooltip("The reference to the menu's blur panel game object")]
    [SerializeField]
    GameObject
        blurPanel = null;
    [Tooltip("Menu elements references")]
    [SerializeField]
    public GameObject
        pauseMenu = null,
        mainMenu = null,
        winScreen = null;

    int playerWhoPaused = 0;

    [Tooltip("The duration during which the players can't input the pause again when they already input it")]
    [SerializeField] float pauseCooldownDuration = 0.1f;
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
    private void Awake()                                                                                // AWAKE
    {
        Instance = this;
    }


    void Start()                                                                                        // START
    {
        // FILLS SCRIPTABLE OBJECTS
        LoadAudioSaveInScriptableObject();
        LoadGameSaveInScriptableObject();


        // FILLS SCENE FROM SCRIPTABLE OBJECTS
        SetUpAudioSettingsFromScriptableObject();
        SetUpGameSettingsFromScriptableObject();


        Cursor.visible = true;
    }


    void Update()                                                                                                   // UPDATE
    {
        if (enabled && isActiveAndEnabled)
        {
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
    void ManageInfosDisplayInput()
    {
        bool playerDead = false;

        for (int i = 0; i < GameManager.Instance.playersList.Count; i++)
        {
            if (GameManager.Instance.playersList[i] != null)
                if (GameManager.Instance.playersList[i].GetComponent<Player>().playerState == Player.STATE.dead)
                    playerDead = true;
        }

        // Display score & in game help
        if (!playerDead && GameManager.Instance.gameState == GameManager.GAMESTATE.game)
        {
            GameManager.Instance.scoreObject.GetComponent<Animator>().SetBool("On", inputManager.scoreInput);


            for (int i = 0; i < GameManager.Instance.playersList.Count; i++)
            {
                // ONLINE
                if (ConnectManager.Instance != null && ConnectManager.Instance.connectedToMaster && i > 0)
                    break;

                GameManager.Instance.inGameHelp[i].SetBool("On", inputManager.playerInputs[i].score);
                if (GameManager.Instance.playersList[i] != null)
                    GameManager.Instance.playersList[i].GetComponent<PlayerAnimations>().nameDisplayAnimator.SetBool("On", inputManager.playerInputs[i].score);
            }
        }
        // Display in game help key indication
        if (!playerDead && GameManager.Instance.gameState == GameManager.GAMESTATE.game)
            for (int i = 0; i < GameManager.Instance.playersList.Count; i++)
                if (GameManager.Instance.playerKeysIndicators[i] != null && inputManager.playerInputs.Length > i)
                    GameManager.Instance.playerKeysIndicators[i].SetBool("On", !inputManager.playerInputs[i].score);
    }
    # endregion






    # region PAUSE
    // PAUSE INPUT
    // Input to activate pause
    void ManagePauseOnInput()
    {
        if (!pauseCooldownOn)
            for (int i = 0; i < inputManager.playerInputs.Length; i++)
                if (inputManager.playerInputs[i].pauseUp)
                {
                    playerWhoPaused = i;
                    pauseCooldownOn = true;
                    pauseCooldownStartTime = Time.time;
                    TriggerPause(true);
                }
    }
    // PAUDE OFF INPUT
    // Input to deactivate pause by the player who activated it only
    void ManagePauseOutInput()
    {
        if (!pauseCooldownOn)
            if (inputManager.playerInputs[playerWhoPaused].pauseUp)
            {
                pauseCooldownOn = false;
                pauseCooldownStartTime = Time.time;
                TriggerPause(false);
            }
    }


    // SWITCH PAUSED
    public void SwitchPause(bool actuallyDoSomething = true)
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
    public void PauseOff()
    {
        TriggerPause(false);
    }


    // PAUSE FUNCTION
    public void TriggerPause(bool state)
    {
        if (state)
        {
            backIndicator.SetActive(true);

            audioManager.SwitchAudioState(AudioManager.AUDIOSTATE.pause);
            GameManager.Instance.SwitchState(GameManager.GAMESTATE.paused);

            if (!ConnectManager.Instance.enableMultiplayer)
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
            audioManager.SwitchAudioState(AudioManager.AUDIOSTATE.pause);

            if (GameManager.Instance.gameState == GameManager.GAMESTATE.paused)
                GameManager.Instance.SwitchState(GameManager.GAMESTATE.game);


            if (GameManager.Instance.playersList != null && GameManager.Instance.playersList.Count > 0)
                for (int i = 0; i < GameManager.Instance.playersList.Count; i++)
                    if (GameManager.Instance.playersList.Count > i && GameManager.Instance.playersList[i] != null)
                        GameManager.Instance.playersList[i].GetComponent<Animator>().enabled = true;

            Invoke("RestoreCanPauseOn", 0.2f);
        }


        blurPanel.SetActive(state);
        pauseMenu.SetActive(state);

        GameManager.Instance.scoreObject.GetComponent<Animator>().SetBool("On", state);

        if (GameManager.Instance.playersList != null && GameManager.Instance.playersList.Count > 0)
            for (int i = 0; i < GameManager.Instance.playersList.Count; i++)
            {
                if (GameManager.Instance.inGameHelp.Length > i && GameManager.Instance.inGameHelp[i] != null)
                    GameManager.Instance.inGameHelp[i].SetBool("On", state);
                
                if (GameManager.Instance.playersList[i] != null)
                    GameManager.Instance.playersList[i].GetComponent<PlayerAnimations>().nameDisplayAnimator.SetBool("On", false);

                if (GameManager.Instance.playerKeysIndicators.Count > i && GameManager.Instance.playerKeysIndicators[i] != null)
                    GameManager.Instance.playerKeysIndicators[i].SetBool("On", !state);
            }


        Cursor.visible = state;
    }


    void RestoreCanPauseOn()
    {
        canPauseOn = true;
    }
    #endregion





    // WIN SCREEN
    public void SetUpWinMenu(string winnerName, Color winnerColor, Vector2 score, Color[] playersColors)
    {
        winName.text = winnerName;
        winName.color = winnerColor;


        for (int i = 0; i < GameManager.Instance.playersList.Count; i++)
        {
            scoresNames[i].text = charactersData.charactersList[GameManager.Instance.playersList[i].GetComponent<Player>().characterIndex].name;
            scoresNames[i].color = playersColors[i];
            scoresDisplays[i].text = score[i].ToString();
            scoresDisplays[i].color = playersColors[i];
        }
    }




    #region SAVE / LOAD PARAMETERS
    // GAME SETTINGS FROM SAVE TO SCRIPTABLE OBJECTS
    public void LoadGameSaveInScriptableObject()
    {
        JsonSave save = SaveGameManager.GetCurrentSave();

        menuParametersSaveScriptableObject.displayHelp = save.displayHelp;
        menuParametersSaveScriptableObject.roundToWin = save.roundsToWin;
    }
    // AUDIO SETTINGS FROM SAVE TO SCRIPTABLE OBJECT
    public void LoadAudioSaveInScriptableObject()
    {
        JsonSave save = SaveGameManager.GetCurrentSave();


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


    // SET UP GAME SETTINGS
    public void SetUpGameSettingsFromScriptableObject()
    {
        roundsToWinSlider.value = menuParametersSaveScriptableObject.roundToWin;
        displayHelpCheckBox.SetActive(menuParametersSaveScriptableObject.displayHelp);


        for (int i = 0; i < GameManager.Instance.inGameHelp.Length; i++)
        {
            GameManager.Instance.inGameHelp[i].gameObject.SetActive(menuParametersSaveScriptableObject.displayHelp);
            GameManager.Instance.playerKeysIndicators[i].gameObject.SetActive(menuParametersSaveScriptableObject.displayHelp);
        }
    }
    // SET UP AUDIO SETTINGS
    public void SetUpAudioSettingsFromScriptableObject()
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
    public void SaveGameSettingsInScriptableObject()
    {
        menuParametersSaveScriptableObject.roundToWin = Mathf.FloorToInt(roundsToWinSlider.value);
        menuParametersSaveScriptableObject.displayHelp = displayHelpCheckBox.activeInHierarchy;
    }
    // SAVE AUDIO SETTINGS
    public void SaveAudioSettingsInScriptableObject()
    {
        menuParametersSaveScriptableObject.masterVolume = masterVolume.slider.value;
        menuParametersSaveScriptableObject.menuMusicVolume = menuMusicVolume.slider.value;
        menuParametersSaveScriptableObject.battleMusicVolume = battleMusicVolume.slider.value;
        menuParametersSaveScriptableObject.menuFXVolume = menuFXVolume.slider.value;
        menuParametersSaveScriptableObject.fxVolume = fxVolume.slider.value;
        menuParametersSaveScriptableObject.voiceVolume = voiceVolume.slider.value;
        menuParametersSaveScriptableObject.roundToWin = GameManager.Instance.scoreToWin;
    }


    // GAME SETTINGS FROM SCRIPTABLE OBJECT TO SAVE
    public void SaveGameSettingsFromScriptableObject()
    {
        JsonSave save = SaveGameManager.GetCurrentSave();

        save.roundsToWin = menuParametersSaveScriptableObject.roundToWin;
        save.displayHelp = menuParametersSaveScriptableObject.displayHelp;

        SaveGameManager.Save();
    }
    // AUDIO SETTINGS FROM SCRIPTABLE OBJECT TO SAVE
    public void SaveAudioSettingsFromScriptableObject()
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
    # endregion
}
