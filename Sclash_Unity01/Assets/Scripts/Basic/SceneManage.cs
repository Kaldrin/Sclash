using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManage : MonoBehaviour {

    //Scene changement
    [SerializeField]
    Animator sceneSwitchAnimator;
    public bool proceedToLoadScene = false;
    Scene sceneToLoad;
    bool quit = false;


    //Restarting scene
    public KeyCode[]
        pressSimultaneousKeysToRestart;



    // Use this for initialization
    void Start () {
        //sceneSwitchAnimator = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

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

    
    //Scene loading

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

    void Quit()
    {
        sceneSwitchAnimator.SetTrigger("CloseScene");
        quit = true;
    }



    //Scene controls

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
