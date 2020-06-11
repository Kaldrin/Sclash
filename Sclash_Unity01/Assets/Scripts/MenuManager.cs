using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;


// Created for Unity 2019.1.1f1
public class MenuManager : MonoBehaviour
{
    # region VARIABLES
    # region MANAGERS
    [Header("MANAGERS")]
    [Tooltip("The references to the GameManager script instance component")]
    [SerializeField] GameManager gameManager = null;

    [Tooltip("The AudioManager script instance reference")]
    [SerializeField] AudioManager audioManager = null;

    [Tooltip("The references to the InputManager script instance component")]
    [SerializeField] InputManager inputManager = null;
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
    // Start is called before the first frame update
    void Start()
    {
        LoadAudioSaveInScriptableObject();
        LoadGameSaveInScriptableObject();
        SetUpAudioSettingsFromScriptableObject();
        SetUpGameSettingsFromScriptableObject();
        Cursor.visible = true;
    }

    // Update is called once per grahic frame
    void Update()
    {
        switch (gameManager.gameState)
        {
            case GameManager.GAMESTATE.game:
                ManagePauseOnInput();
                ManageInfosDisplayInput();
                break;

            case GameManager.GAMESTATE.paused:
                ManagePauseOutInput();
                break;
        }


        if (pauseCooldownOn && Time.time - pauseCooldownStartTime > pauseCooldownDuration)
        {
            pauseCooldownOn = false;
        }
    }
    # endregion






    # region SCORE DISPLAY IN GAME
    // Update info display
    void ManageInfosDisplayInput()
    {
        bool playerDead = false;

        for (int i = 0; i < gameManager.playersList.Count; i++)
        {
            if (gameManager.playersList[i] != null)
            {
                if (gameManager.playersList[i].GetComponent<Player>().playerState == Player.STATE.dead)
                {
                    playerDead = true;
                }
            }
        }

        // Display score & in game help
        if (!playerDead && gameManager.gameState == GameManager.GAMESTATE.game)
        {
            gameManager.scoreObject.GetComponent<Animator>().SetBool("On", inputManager.scoreInput);


            for (int i = 0; i < gameManager.playersList.Count; i++)
            {
                if (ConnectManager.Instance.connectedToMaster && i > 0)
                    break;

                gameManager.inGameHelp[i].SetBool("On", inputManager.playerInputs[i].score);
                if (gameManager.playersList[i] != null)
                    gameManager.playersList[i].GetComponent<PlayerAnimations>().nameDisplayAnimator.SetBool("On", inputManager.playerInputs[i].score);
            }
        }
        // Display in game help key indication
        if (!playerDead && audioManager.audioState == AudioManager.AUDIOSTATE.battle)
        {
            for (int i = 0; i < gameManager.playersList.Count; i++)
            {
                gameManager.playerKeysIndicators[i].SetBool("On", !inputManager.playerInputs[i].score);
            }
        }
    }
    # endregion






    # region PAUSE
    // Input to activate pause
    void ManagePauseOnInput()
    {
        if (!pauseCooldownOn)
        {
            for (int i = 0; i < inputManager.playerInputs.Length; i++)
            {
                if (inputManager.playerInputs[i].pauseUp)
                {
                    playerWhoPaused = i;
                    pauseCooldownOn = true;
                    pauseCooldownStartTime = Time.time;
                    TriggerPause(true);
                }
            }
        }
    }

    // Input to deactivate pause by the player who activated it only
    void ManagePauseOutInput()
    {
        if (!pauseCooldownOn)
        {
            if (inputManager.playerInputs[playerWhoPaused].pauseUp)
            {
                pauseCooldownOn = false;
                pauseCooldownStartTime = Time.time;
                TriggerPause(false);
            }
        }
    }

    public void SwitchPause()
    {
        if (gameManager.gameState == GameManager.GAMESTATE.paused)
            TriggerPause(false);
        else
            TriggerPause(true);
    }

    public void TriggerPause(bool state)
    {
        if (state)
        {
            audioManager.SwitchAudioState(AudioManager.AUDIOSTATE.pause);
            gameManager.SwitchState(GameManager.GAMESTATE.paused);
        }
        else
        {
            audioManager.SwitchAudioState(AudioManager.AUDIOSTATE.pause);
            gameManager.SwitchState(GameManager.GAMESTATE.game);
        }


        blurPanel.SetActive(state);
        pauseMenu.SetActive(state);

        gameManager.scoreObject.GetComponent<Animator>().SetBool("On", state);


        for (int i = 0; i < gameManager.playersList.Count; i++)
        {
            gameManager.inGameHelp[i].SetBool("On", state);
            gameManager.playersList[i].GetComponent<PlayerAnimations>().nameDisplayAnimator.SetBool("On", false);
            gameManager.playerKeysIndicators[i].SetBool("On", !state);
        }


        Cursor.visible = state;
    }
    #endregion






    public void SetUpWinMenu(string winnerName, Color winnerColor, Vector2 score, Color[] playersColors)
    {
        winName.text = winnerName;
        winName.color = winnerColor;


        for (int i = 0; i < gameManager.playersList.Count; i++)
        {
            scoresNames[i].text = charactersData.charactersList[gameManager.playersList[i].GetComponent<Player>().characterIndex].name;
            scoresNames[i].color = playersColors[i];
            scoresDisplays[i].text = score[i].ToString();
            scoresDisplays[i].color = playersColors[i];
        }
    }




    #region SAVE / LOAD PARAMETERS
    public void LoadGameSaveInScriptableObject()
    {
        JsonSave save = SaveGameManager.GetCurrentSave();

        menuParametersSaveScriptableObject.displayHelp = save.displayHelp;
        menuParametersSaveScriptableObject.roundToWin = save.roundsToWin;
    }

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

    public void SetUpGameSettingsFromScriptableObject()
    {
        roundsToWinSlider.value = menuParametersSaveScriptableObject.roundToWin;
        displayHelpCheckBox.SetActive(menuParametersSaveScriptableObject.displayHelp);


        for (int i = 0; i < gameManager.inGameHelp.Length; i++)
        {
            gameManager.inGameHelp[i].gameObject.SetActive(menuParametersSaveScriptableObject.displayHelp);
            gameManager.playerKeysIndicators[i].gameObject.SetActive(menuParametersSaveScriptableObject.displayHelp);
        }
    }

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

    public void SaveGameSettingsInScriptableObject()
    {
        menuParametersSaveScriptableObject.roundToWin = Mathf.FloorToInt(roundsToWinSlider.value);
        menuParametersSaveScriptableObject.displayHelp = displayHelpCheckBox.activeInHierarchy;
    }

    public void SaveAudioSettingsInScriptableObject()
    {
        menuParametersSaveScriptableObject.masterVolume = masterVolume.slider.value;
        menuParametersSaveScriptableObject.menuMusicVolume = menuMusicVolume.slider.value;
        menuParametersSaveScriptableObject.battleMusicVolume = battleMusicVolume.slider.value;
        menuParametersSaveScriptableObject.menuFXVolume = menuFXVolume.slider.value;
        menuParametersSaveScriptableObject.fxVolume = fxVolume.slider.value;
        menuParametersSaveScriptableObject.voiceVolume = voiceVolume.slider.value;
        menuParametersSaveScriptableObject.roundToWin = gameManager.scoreToWin;
    }

    public void SaveGameSettingsFromScriptableObject()
    {
        JsonSave save = SaveGameManager.GetCurrentSave();

        save.roundsToWin = menuParametersSaveScriptableObject.roundToWin;
        save.displayHelp = menuParametersSaveScriptableObject.displayHelp;

        SaveGameManager.Save();
    }

    // Save menu parameters
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
