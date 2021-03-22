using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using UnityEngine.InputSystem;




// HEADER
// OPTIMIZED I THINK ?
// Reusable script

// REQUIREMENTS
// Input System package
// Photon Unity package

/// <summary>
/// This script manages scene transitions & scene loading. It requires an animator to animate the transition
/// </summary>

// VERSION
// Created for Unity 2019.1.1f1
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
    int index = 0;
    public bool proceedToLoadScene = false;
    bool quit = false;
    bool canLoadScene = true;
    bool loadWithIndex = false;

    [Header("RESTART SCENE")]
    [SerializeField] bool allowSceneRestartInBuild = false;
    [Tooltip("Choose which keys should be pressed to restart the scene")]
    PlayerControls controls;









    #region FUNCTIONS
    #region BASE FUNCTIONS
    void Awake()                                                                                                         // AWAKE
    {
        Instance = this;
        GetComponents();
    }


    void Start()                                                                                                             // START
    {
        controls = new PlayerControls();
        // If chosen, starts the coroutine that will load the indicated scene after the indicated duration
        if (autoLoadSceneAfterDuration)
            Invoke("AutoLoadSceneAfterDuration", durationBeforeAutoLoadScene);
    }


    void Update()                                                                                                              // UPDATE
    {
        if (isActiveAndEnabled && enabled)
        {
            // Checks if the inputs to restart the scene were pressed
            if (Application.isEditor || allowSceneRestartInBuild)
                if (controls.Menu.Restart.triggered)
                    Restart();


            // Is called when the scene switch screen finished fading on
            if (proceedToLoadScene)
            {
                if (quit)
                    Application.Quit();
                else if (canLoadScene)
                {
                    canLoadScene = false;

                    if (loadWithIndex)
                        LoadScene(index);
                    else
                        LoadScene(sceneToLoad);
                }
            }
        }
    }
    #endregion




    // SCENE LOADING
    // Automatically loads the indicated scene after the indicated duration, without transition animation
    void AutoLoadSceneAfterDuration()                                                                                                   // AUTO LOAD SCENE AFTER DURATION
    {
        SceneManager.LoadSceneAsync(sceneToAutoLoadIndex);
    }

    void LoadScene(Scene scene)                                                                                                             // LOAD SCENE
    {
        SceneManager.LoadSceneAsync(scene.name);
    }


    // Proceeds to actually load the scene
    void LoadScene(int index)                                                                                                               // LOAD SCENE
    {
        SceneManager.LoadScene(index);
    }


    // Set which scene should be loaded after the close scene anim
    public void SetLoadScene(int sceneIndex)                                                                                                // SET LOAD SCENE
    {
        loadWithIndex = true;
        index = sceneIndex;
        sceneSwitchAnimator.SetTrigger("CloseScene");
    }


    // Set which scene should be loaded after the close scene anim
    public void SetLoadScene(Scene scene)                                                                                                   // SET LOAD SCENE
    {
        sceneToLoad = scene;
        sceneSwitchAnimator.SetTrigger("CloseScene");
    }


    // Triggers the restart of the current scene, called by the restart inputs
    public void Restart()                                                                                                                           // RESTART
    {
        SetLoadScene(SceneManager.GetActiveScene());
    }








    // QUIT
    // Sets the instruction to quit the game after after the close scene anim
    public void Quit()                                                                                                                                      // QUIT
    {
        PhotonNetwork.Disconnect();
        SetLoadScene(new Scene());
        quit = true;
    }













    // SECONDARY
    // Checks if the given keys are being pressed
    bool CheckIfAllKeysPressed(KeyCode[] keys)                                                                                                  // CHECK IF ALL KEYS PRESSED
    {
        //bool notPressed = false;


        /* for (int i = 0; i < keys.Length; i++)
             if (!Input.GetKey(keys[i]))
                 notPressed = true;
        */

        bool notPressed = true;
        return !notPressed;
    }


    private static string NameFromIndex(int index)                                                                                            // NAME FROM INDEX
    {
        string scenePath = SceneUtility.GetScenePathByBuildIndex(index);

        string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
        return sceneName;
    }


    // Checks if it's possible to automatically find the missing components references
    void GetComponents()                                                                                                                            // GET COMPONENTS
    {
        if (sceneSwitchAnimator == null && GetComponent<Animator>())
            sceneSwitchAnimator = GetComponent<Animator>();
    }







    // EDITOR
    // Automatically get components references, ergonomy
    private void OnDrawGizmosSelected()
    {
        GetComponents();
    }
    #endregion
}
