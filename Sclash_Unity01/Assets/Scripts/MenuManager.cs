using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

    // SAVE PARAMETERS
    [Header("Save parameters")]
    [SerializeField] MenuParameters menuParametersSave = null;
    [SerializeField] SliderToVolume
        masterVolume = null,
        musicVolume = null,
        fxVolume = null,
        voiceVolume = null;
    //[SerializeField] DynamicValueTMP roundToWin = null;





    // PAUSE
    [Header("Pause menu")]
    [SerializeField] GameObject
        blurPanel = null;
    [SerializeField] public GameObject
        pauseMenu = null,
        mainMenu = null;
    [SerializeField] GameObject
        backButton = null,
        resumeButton = null,
        quitButton = null,
        mainMenuButton = null;





    // MENUS ITEMS
    [Header("Menu items")]
    [SerializeField] int mainMenuDefaultIndex = 2;
    [SerializeField] int optionsMenuDefaultIndex = 0;
    /*
    int
        currentMainMenuIndex = 0,
        currentOptionsMenuIndex = 0;
        */


    [SerializeField] MenuElement[]
        mainMenuItems = null,
        optionsMenuItems = null;

    //[SerializeField] MenuElement[] currentMenu = null;






    // GAME MANAGER
    [Header("Game manager")]
    [SerializeField] string gameManagerName = "GlobalManager";
    GameManager gameManager;










    // BASE FUNCTIONS
    // Start is called before the first frame update
    void Start()
    {
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


        MenuManage();
    }






    // MENU MANAGE
    // Manage menu navigation
    void MenuManage()
    {
        if (mainMenu.activeSelf)
        {
        }


        if (pauseMenu.activeSelf)
        {

        }
    }

    // Select default main menu element
    void DefaultMain()
    {
        mainMenuItems[mainMenuDefaultIndex].Select(true);
    }

    // Select default options menu element
    void DefaultOptions()
    {
        optionsMenuItems[optionsMenuDefaultIndex].Select(true);
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
    }

    // Save menu parameters
    public void SaveParameters()
    {
        // Save for current session
        menuParametersSave.masterVolume = masterVolume.slider.value;
        menuParametersSave.musicVolume = musicVolume.slider.value;
        menuParametersSave.fxVolume = fxVolume.slider.value;
        menuParametersSave.voiceVolume = voiceVolume.slider.value;


        // Save forever
        JsonSave save = SaveGameManager.GetCurrentSave();

        save.masterVolume = masterVolume.slider.value;
        save.musicVolume = musicVolume.slider.value;
        save.fxVolume = fxVolume.slider.value;
        save.voiceVolume = voiceVolume.slider.value;

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

        gameManager.paused = state;

        SaveParameters();
    }
}
