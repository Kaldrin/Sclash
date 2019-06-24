using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


// Created for Unity 2019.1.1f1
public class SceneManage : MonoBehaviour {

    //Scene changement
    [Header("SCENE CHANGE")]
    [SerializeField] Animator sceneSwitchAnimator = null;
    public bool proceedToLoadScene = false;
    Scene sceneToLoad;
    bool quit = false;



    //Restarting scene
    [Header("RESTART SCENE")]
    public KeyCode[]
        pressSimultaneousKeysToRestart;









    // BASE FUNCTIONS
    // Use this for initialization
    void Start () {
        //sceneSwitchAnimator = GetComponent<Animator>();
    }
	
	// Update is called once per graphic frame
	void Update () {
		
	}

    // FixedUpdate is called 30 times per second
    void FixedUpdate()
    {
        //Checks if the conditions to restart the scene were fulfilled
        if (CheckIfAllKeysPressed(pressSimultaneousKeysToRestart))
        {
            Restart();
        }


        //Is called when the scene switch screen finished fading on
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
    void LoadScene(Scene scene)
    {
        SceneManager.LoadScene(scene.name);
    }
    
    void Restart()
    {
        
        sceneToLoad = SceneManager.GetActiveScene();
        sceneSwitchAnimator.SetTrigger("CloseScene");
        
    }

    public void SetLoadScene(Scene scene)
    {
        sceneToLoad = scene;
        sceneSwitchAnimator.SetTrigger("CloseScene");
    }

    public void Quit()
    {
        sceneSwitchAnimator.SetTrigger("CloseScene");
        quit = true;
    }







    // SCENE CONTROLS
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
