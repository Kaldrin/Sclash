using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;


// Created for Unity 2019.1.1f1
public class MenuManager : MonoBehaviour
{
    # region FUNCTIONS
    # region MANAGERS
    // MANAGERS
    [Header("MANAGERS")]
    // Game managers
    [Tooltip("The references to the GameManager script instance component")]
    [SerializeField] GameManager gameManager = null;

    // Input manager
    [Tooltip("The references to the InputManager script instance component")]
    [SerializeField] InputManager inputManager = null;
    # endregion






    # region SAVE PARAMETERS
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
                ManageScoreDisplayInput();
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






    # region SCORE
    // SCORE
    //Update score display
    void ManageScoreDisplayInput()
    {
        bool playerDead = false;

        for (int i = 0; i < gameManager.playersList.Count; i++)
        {
            if (gameManager.playersList[i].GetComponent<Player>().playerState == Player.STATE.dead)
            {
                playerDead = true;
            }
        }


        if (!playerDead && gameManager.gameState == GameManager.GAMESTATE.game && gameManager.gameState == GameManager.GAMESTATE.game)
        {
            //gameManager.scoreObject.SetActive(inputManager.scoreInput);
            gameManager.scoreObject.GetComponent<Animator>().SetBool("On", inputManager.scoreInput);
        }
    }
    # endregion






    # region PAUSE
    // PAUSE
    // Input to activate pause
    void ManagePauseOnInput()
    {
        if (!pauseCooldownOn)
        {
            Debug.Log("Can pause");
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

    void TriggerPause(bool state)
    {
        if (state)
            gameManager.SwitchState(GameManager.GAMESTATE.paused);
        else
            gameManager.SwitchState(GameManager.GAMESTATE.game);


        blurPanel.SetActive(state);
        pauseMenu.SetActive(state);
        //gameManager.scoreObject.SetActive(state);
        gameManager.scoreObject.GetComponent<Animator>().SetBool("On", state);

        //backButton.SetActive(!state);
        //resumeButton.SetActive(state);
        //quitButton.SetActive(state);
        //mainMenuButton.SetActive(state);
        //changeMapButton.SetActive(false);
        //scoreToWinSlider.SetActive(false);
        //backIndicator.SetActive(state);

        //pauseOptionMenuBrowser.SetActive(state);
        //mainOptionMenuBrowser.SetActive(false);
        

        Cursor.visible = state;


        /*
        for (int i = 0; i < gameManager.playersList.Count; i++)
        {
            if (state)
                gameManager.playersList[i].GetComponent<Animator>().speed = 0;
            else
                gameManager.playersList[i].GetComponent<Animator>().speed = 1;
        }
        */


        //SaveParameters();
    }
    # endregion






    # region SAVE
    // SAVE / LOAD PARAMETERS
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
