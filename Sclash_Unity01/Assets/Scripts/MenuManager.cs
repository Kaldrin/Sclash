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






    #region SAVE PARAMETERS
    // SAVE PARAMETERS
    [Header("SAVE PARAMETERS")] [SerializeField] MenuParameters menuParametersSave = null;
    [Tooltip("The references to the Sliders components in the options menu controlling the volumes of the different tracks of the game")]
    [SerializeField] SliderToVolume
        masterVolume = null,
        musicVolume = null,
        menuFXVolume = null,
        fxVolume = null,
        voiceVolume = null;
    [Tooltip("The reference to the Slider component in the options menu controlling the number of rounds needed to win the game")]
    [SerializeField] Slider roundsToWinSlider = null;
    # endregion






    # region PAUSE
    // PAUSE
    [Header("PAUSE MENU")]
    [Tooltip("The reference to the menu's blur panel game object")]
    [SerializeField] GameObject
        blurPanel = null;
    [Tooltip("Menu elements references")]
    [SerializeField] public GameObject
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
        LoadParameters();
        Cursor.visible = true;
    }

    // Update is called once per grahic frame
    void Update()
    {
        switch(gameManager.gameState)
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
    //Update score display
    void ManageInfosDisplayInput()
    {
        bool playerDead = false;

        for (int i = 0; i < gameManager.playersList.Count; i++)
        {
            if (gameManager.playersList[i].GetComponent<Player>().playerState == Player.STATE.dead)
            {
                playerDead = true;
            }
        }

        // Display score & in game help
        if (!playerDead && gameManager.gameState == GameManager.GAMESTATE.game)
        {
            gameManager.scoreObject.GetComponent<Animator>().SetBool("On", inputManager.scoreInput);
            

            for (int i = 0; i < gameManager.playersList.Count; i++)
            {
                gameManager.inGameHelp[i].SetBool("On", inputManager.playerInputs[i].score);
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
                pauseCooldownOn = true;
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
    # endregion




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






    # region SAVE / LOAD PARAMETERS
    // Load menu parameters
    void LoadParameters()
    {  
        
        // Save from current session saves
        masterVolume.slider.value = menuParametersSave.masterVolume;
        masterVolume.UpdateVolume();

        musicVolume.slider.value = menuParametersSave.musicVolume;
        musicVolume.UpdateVolume();

        menuFXVolume.slider.value = menuParametersSave.menuFXVolume;
        menuFXVolume.UpdateVolume();

        fxVolume.slider.value = menuParametersSave.fxVolume;
        fxVolume.UpdateVolume();

        voiceVolume.slider.value = menuParametersSave.voiceVolume;
        voiceVolume.UpdateVolume();

        roundsToWinSlider.value = menuParametersSave.roundToWin;
        
        


        // Loads actual saves
        JsonSave save = SaveGameManager.GetCurrentSave();

        masterVolume.slider.value = save.masterVolume;
        masterVolume.UpdateVolume();

        musicVolume.slider.value = save.musicVolume;
        musicVolume.UpdateVolume();

        menuFXVolume.slider.value = save.menuFXVolume;
        menuFXVolume.UpdateVolume();

        fxVolume.slider.value = save.fxVolume;
        fxVolume.UpdateVolume();

        voiceVolume.slider.value = save.voiceVolume;
        voiceVolume.UpdateVolume();

        roundsToWinSlider.value = save.roundsToWin;
    }

    // Save menu parameters
    public void SaveParameters()
    {
        // Save for current session
        menuParametersSave.masterVolume = masterVolume.slider.value;
        menuParametersSave.musicVolume = musicVolume.slider.value;
        menuParametersSave.menuFXVolume = menuFXVolume.slider.value;
        menuParametersSave.fxVolume = fxVolume.slider.value;
        menuParametersSave.voiceVolume = voiceVolume.slider.value;
        menuParametersSave.roundToWin = gameManager.scoreToWin;



        // Save forever
        JsonSave save = SaveGameManager.GetCurrentSave();


        save.masterVolume = masterVolume.slider.value;
        save.musicVolume = musicVolume.slider.value;
        save.menuFXVolume = menuFXVolume.slider.value;
        save.fxVolume = fxVolume.slider.value;
        save.voiceVolume = voiceVolume.slider.value;
        save.roundsToWin = Mathf.FloorToInt(roundsToWinSlider.value);

        SaveGameManager.Save();
    }
    #endregion



    # endregion
}
