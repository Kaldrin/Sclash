using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MenuManager : MonoBehaviour
{
    // PAUSE
    [Header("Pause menu")]
    [SerializeField] GameObject
        blurPanel = null;
    [SerializeField] GameObject pauseMenu = null,
        backButton = null,
        resumeButton = null;



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
        gameManager.paused = state;

        if (state)
        {
        }
        else
        {

        }
    }
}
