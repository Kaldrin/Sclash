using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    // SAVE PARAMETERS
    [Header("Save parameters")] [SerializeField] MenuParameters menuParametersSave = null;
    [SerializeField] SliderToVolume
        masterVolume = null,
        musicVolume = null,
        fxVolume = null,
        voiceVolume = null;
    [SerializeField] DynamicValueTMP roundToWin = null;





    // PAUSE
    [Header("Pause menu")]
    [SerializeField] GameObject
        blurPanel = null;
    [SerializeField]
    public GameObject
        pauseMenu = null,
        mainMenu = null,
        winScreen = null;
    [SerializeField] GameObject
        backButton = null,
        resumeButton = null,
        quitButton = null,
        mainMenuButton = null;



    // WIN
    [Header("Win menu")] [SerializeField] GameObject backToWinScreenButton = null;
    [SerializeField] public GameObject winMessage = null;
    [SerializeField] public TextMeshProUGUI winName = null;



    // MANAGERS
    [Header("Managers")]
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

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonUp("Pause"))
        {
            if (gameManager.gameStarted)
            {
                TriggerPause(!gameManager.paused);
            }
        }

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

        roundToWin.value = menuParametersSave.roundToWin;




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

        roundToWin.value = save.roundsToWin;
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
        save.roundsToWin = roundToWin.value;

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
        backToWinScreenButton.SetActive(false);

        gameManager.paused = state;

        SaveParameters();
    }
}
