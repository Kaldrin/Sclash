using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

// Created for Unity 2019.1.1f1
// This script manages scene transitions and scene loading
// OPTIMIZED I THINK ?
public class SceneManage : MonoBehaviour
{
    public static SceneManage Instance;


    [Header("SCENE CHANGE")]
    [Tooltip("Should this script automatically load a referenced scene after a certain amount of time ?")]
    [SerializeField] bool autoLoadSceneAfterDuration = false;
    [SerializeField] float durationBeforeAutoLoadScene = 0.5f;
    [SerializeField] int sceneToAutoLoadIndex = 1;
    [SerializeField] Animator sceneSwitchAnimator = null;

    Scene sceneToLoad;
    public bool proceedToLoadScene = false;
    bool quit = false;
    bool canLoadScene = true;



    [Header("RESTART SCENE")]
    [Tooltip("Choose which keys should be pressed to restart the scene")]
    [SerializeField] KeyCode[] pressSimultaneousKeysToRestart = null;











    #region FUNCTIONS
    #region BASE FUNCTIONS
    void Awake()
    {
        Instance = this;
    }


    // Use this for initialization
    void Start()
    {
        // If chosen, starts the coroutine that will load the indicated scene after the indicated duration
        if (autoLoadSceneAfterDuration)
            StartCoroutine(AutoLoadSceneAfterDuration());
    }


    // FixedUpdate is called 50 times per second
    void Update()
    {
        if (isActiveAndEnabled && enabled)
        {
            // Checks if the inputs to restart the scene were pressed
            if (CheckIfAllKeysPressed(pressSimultaneousKeysToRestart))
                Restart();


            // Is called when the scene switch screen finished fading on
            if (proceedToLoadScene)
            {
                if (quit)
                    Application.Quit();
                else if (canLoadScene)
                {
                    canLoadScene = false;
                    LoadScene(sceneToLoad);
                }
            }
        }
    }
    #endregion




    // SCENE LOADING
    // Automaticly loads the indicated scene after the indicated duration, without transition animation
    IEnumerator AutoLoadSceneAfterDuration()
    {
        yield return new WaitForSecondsRealtime(durationBeforeAutoLoadScene);


        SceneManager.LoadSceneAsync(sceneToAutoLoadIndex);
    }

    void LoadScene(Scene scene)
    {
        SceneManager.LoadSceneAsync(scene.name);
    }

    // Set which scene should be loaded after the close scene anim
    public void SetLoadScene(Scene scene)
    {
        sceneToLoad = scene;
        sceneSwitchAnimator.SetTrigger("CloseScene");
    }

    // Triggers the restart of the current scene, called by the restart inputs
    public void Restart()
    {
        SetLoadScene(SceneManager.GetActiveScene());
    }








    // QUIT
    // Sets the instruction to quit the game after after the close scene anim
    public void Quit()
    {
        PhotonNetwork.Disconnect();
        SetLoadScene(new Scene());
        quit = true;
    }













    // SECONDARY
    // Checks if the given keys are being pressed
    bool CheckIfAllKeysPressed(KeyCode[] keys)
    {
        bool notPressed = false;


        for (int i = 0; i < keys.Length; i++)
        {
            if (!Input.GetKey(keys[i]))
                notPressed = true;
        }


        return !notPressed;
    }
    #endregion
}
