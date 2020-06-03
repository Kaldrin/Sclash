using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

// Created for Unity 2019.1.1f1
public class SceneManage : MonoBehaviour
{

    // SCENE CHANGE
    [Header("SCENE CHANGE")]
    [SerializeField] bool autoLoadSceneAfterDuration = false;
    public bool proceedToLoadScene = false;
    bool quit = false;

    [SerializeField] float durationBeforeAutoLoadScene = 0.5f;

    [SerializeField] int sceneToAutoLoadIndex = 1;

    [SerializeField] Animator sceneSwitchAnimator = null;

    Scene sceneToLoad;






    // RESTART SCENE
    [Header("RESTART SCENE")]
    public KeyCode[]
        pressSimultaneousKeysToRestart;













    // BASE FUNCTIONS
    // Use this for initialization
    void Start()
    {
        // If chosen, starts the coroutine that will load the indicated scene after the indicated duration
        if (autoLoadSceneAfterDuration)
            StartCoroutine(AutoLoadSceneAfterDuration());
    }

    // Update is called once per graphic frame
    void Update()
    {

    }

    // FixedUpdate is called 50 times per second
    void FixedUpdate()
    {
        // Checks if the inputs to restart the scene were pressed
        if (CheckIfAllKeysPressed(pressSimultaneousKeysToRestart))
        {
            Restart();
        }


        // Is called when the scene switch screen finished fading on
        if (proceedToLoadScene)
        {
            if (quit)
            {
                Application.Quit();
            }
            else
                LoadScene(sceneToLoad);
        }
    }





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
    void Restart()
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
            {
                notPressed = true;
            }
        }

        if (notPressed)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
