using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManage : MonoBehaviour {

    // SCENE LOADING
    [Header("Scene loading")]
    [SerializeField] Animator sceneSwitchAnimator = null;
    [SerializeField]  public bool proceedToLoadScene = false;
    Scene sceneToLoad = new Scene();
    bool quit = false;



    // RESTARTING
    [Header("Restarting")]
    [SerializeField] KeyCode[] pressSimultaneousKeysToRestart = null;












    // BASE FUNCTIONS
    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
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
    // Open the selected scene
    void LoadScene(Scene scene)
    {
        SceneManager.LoadScene(scene.name);
    }
    
    // Restart the current scene
    public void Restart()
    {
        
        sceneToLoad = SceneManager.GetActiveScene();
        sceneSwitchAnimator.SetTrigger("CloseScene");
        
    }

    // Set to load the selected scene when the transition animation is complete
    public void SetLoadScene(Scene scene)
    {
        sceneToLoad = scene;
        sceneSwitchAnimator.SetTrigger("CloseScene");
    }

    // Quit the game when the transition animation is done
    public void Quit()
    {
        sceneSwitchAnimator.SetTrigger("CloseScene");
        quit = true;
    }








    // SCENE CONTROLS
    // Check if all the keys in the list are pressed simultaneously
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
