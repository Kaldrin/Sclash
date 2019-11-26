using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;


// Created for Unity 2019.1.1f1
public class MenuManager : MonoBehaviour
{
    // SAVE PARAMETERS
    [Header("SAVE PARAMETERS")] [SerializeField] MenuParameters menuParametersSave = null;
    [SerializeField] SliderToVolume
        masterVolume = null,
        musicVolume = null,
        fxVolume = null,
        voiceVolume = null;
    [SerializeField] Slider roundsToWinSlider = null;





    // PAUSE
    [Header("PAUSE MENU")]
    [SerializeField] GameObject
        blurPanel = null;
    [SerializeField] public GameObject
        pauseMenu = null,
        mainMenu = null,
        winScreen = null;
    [SerializeField] GameObject
        backButton = null,
        resumeButton = null,
        quitButton = null,
        mainMenuButton = null,
        pauseOptionMenuBrowser = null,
        mainOptionMenuBrowser = null,
        changeMapButton = null,
        backIndicator = null;

    int playerWhoPaused = 0;

    [SerializeField] float pauseCooldownDuration = 0.1f;
    float pauseCooldownStartTime = 0f;

    bool pauseCooldownOn = false;




    // WIN
    [Header("WIN MENU")]
    [SerializeField] public GameObject winMessage = null;
    [SerializeField] public TextMeshProUGUI winName = null;




    // MANAGERS
    [Header("MANAGERS")]

    // Game managers
    [SerializeField] string gameManagerName = "GlobalManager";
    GameManager gameManager;

    // Input manager
    [SerializeField] string inputManagerName = "GlobalManager";
    InputManager inputManager = null;














    // BASE FUNCTIONS
    // Start is called before the first frame update
    void Start()
    {
        // MANAGER
        // Get the input manager
        inputManager = GameObject.Find(inputManagerName).GetComponent<InputManager>();
        // Get game manager to use in the script
        gameManager = GameObject.Find(gameManagerName).GetComponent<GameManager>();

        //DefaultMain();
        LoadParameters();
    }

    // Update is called once per grahic frame
    void Update()
    {
        /*
        if (Input.GetButtonUp("Pause"))
        {
            if (gameManager.gameStarted && !gameManager.playerDead)
            {
                
            }
        }
        */


        if (gameManager.gameStarted)
        {
            for (int i = 0; i < gameManager.playersList.Count; i++)
            {
                if (!pauseCooldownOn)
                {
                    if (gameManager.paused && inputManager.playerInputs[playerWhoPaused].pauseUp)
                    {
                        TriggerPause(!gameManager.paused);
                        Debug.Log("Unpause");
                        pauseCooldownOn = true;
                        pauseCooldownStartTime = Time.time;
                    }
                    else if (!gameManager.paused && inputManager.playerInputs[i].pauseUp)
                    {
                        TriggerPause(!gameManager.paused);
                        playerWhoPaused = i;
                        Debug.Log("Pause");
                        pauseCooldownOn = true;
                        pauseCooldownStartTime = Time.time;
                    }
                }
            }

            if (pauseCooldownOn && Time.time - pauseCooldownStartTime > pauseCooldownDuration)
            {
                pauseCooldownOn = false;
            }
        }
        


        Debug.Log(playerWhoPaused);


        UpdateScoreDisplay();
    }






    // SCORE
    //Update score display
    void UpdateScoreDisplay()
    {
        bool playerDead = false;

        for (int i = 0; i < gameManager.playersList.Count; i++)
        {
            if (gameManager.playersList[i].GetComponent<PlayerStats>().dead)
            {
                playerDead = true;
            }
        }

        if (!playerDead && !gameManager.paused && gameManager.gameStarted)
            gameManager.scoreObject.SetActive(inputManager.score);
    }






    // SAVE / LOAD PARAMETERS
    // Load menu parameters
    void LoadParameters()
    {
        // Save from current session saves
        masterVolume.slider.value = menuParametersSave.masterVolume;
        masterVolume.UpdateVolume();

        musicVolume.slider.value = menuParametersSave.musicVolume;
        musicVolume.UpdateVolume();

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
        menuParametersSave.fxVolume = fxVolume.slider.value;
        menuParametersSave.voiceVolume = voiceVolume.slider.value;
        menuParametersSave.roundToWin = gameManager.scoreToWin;


        // Save forever
        JsonSave save = SaveGameManager.GetCurrentSave();


        save.masterVolume = masterVolume.slider.value;
        save.musicVolume = musicVolume.slider.value;
        save.fxVolume = fxVolume.slider.value;
        save.voiceVolume = voiceVolume.slider.value;
        save.roundsToWin = Mathf.FloorToInt(roundsToWinSlider.value);

        SaveGameManager.Save();
    }









    // PAUSE
    public void SwitchPause()
    {
        TriggerPause(!gameManager.paused);
    }

    void TriggerPause(bool state)
    {
        blurPanel.SetActive(state);
        pauseMenu.SetActive(state);
        backButton.SetActive(!state);
        resumeButton.SetActive(state);
        quitButton.SetActive(state);
        mainMenuButton.SetActive(state);
        gameManager.scoreObject.SetActive(state);
        changeMapButton.SetActive(false);
        //backIndicator.SetActive(state);

        pauseOptionMenuBrowser.SetActive(state);
        mainOptionMenuBrowser.SetActive(false);

        gameManager.paused = state;
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

        SaveParameters();
    }
}
