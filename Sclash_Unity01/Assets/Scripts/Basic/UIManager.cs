using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {


    Animator uiAnimator = null;

    // ACTIVATING PAUSE MENU
    [SerializeField] KeyCode pauseKey = KeyCode.P;
    [SerializeField] GameObject[] objectsToFreezeOnPause  = null;
    [SerializeField] MonoBehaviour[] scriptsToFreezeOnPause = null;
    [SerializeField] MonoBehaviour[] scriptsToCallOnPause = null;
    [SerializeField] string[] functionsNamesToCallOnPause = null;



    // DEACTIVATING PAUSE MENU
    [SerializeField] MonoBehaviour[] scriptsToCallOnUnpause = null;
    [SerializeField] string[] functionsNamesToCallOnUnpause = null;




    //Manage cursor
    public bool cursorLocked = false;


    // Use this for initialization
    void Start () {
        uiAnimator = GetComponent<Animator>();
        lockCursor(cursorLocked);
    }
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp(pauseKey))
        {
            Pause();
        }
	}


    //Pause

    void Pause()
    {
        if (uiAnimator.GetBool("Pause"))
        {
            uiAnimator.SetBool("Pause", false);
            freezeObjects(true);
            lockCursor(cursorLocked);
            callFunctionsOnUnpause();

        }
        else
        {
            uiAnimator.SetBool("Pause", true);
            freezeObjects(false);
            lockCursor(false);  
            callFunctionsOnPause();
        }
    }



    void callFunctionsOnPause()
    {
        for (int i = 0; i < scriptsToCallOnPause.Length; i++)
        {
            if (functionsNamesToCallOnPause.Length >= i)
            {
                scriptsToCallOnPause[i].Invoke(functionsNamesToCallOnPause[i], 0);
            }
        }
    }

    void callFunctionsOnUnpause()
    {
        for (int i = 0; i < scriptsToCallOnUnpause.Length; i++)
        {
            if (functionsNamesToCallOnUnpause.Length >= i)
            {
                scriptsToCallOnUnpause[i].Invoke(functionsNamesToCallOnUnpause[i], 0);
            }
        }
    }


    //Control cursor

    void lockCursor(bool state)
    {
        if (state)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;

        Cursor.visible = !state;
    }


    //Freeze objects

    void freezeObjects(bool state)
    {
        for (int i = 0; i < objectsToFreezeOnPause.Length; i++)
        {
            objectsToFreezeOnPause[i].SetActive(state);
        }

        for (int i = 0; i < scriptsToFreezeOnPause.Length; i++)
        {
            scriptsToFreezeOnPause[i].enabled = state;
        }
    }
}
