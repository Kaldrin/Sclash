using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {


    Animator uiAnimator;

    //Activating pause menu
    [SerializeField]
    KeyCode pauseKey;
    [SerializeField]
    GameObject[] objectsToFreezeOnPause;
    [SerializeField]
    MonoBehaviour[] scriptsToFreezeOnPause;
    [SerializeField]
    MonoBehaviour[] scriptsToCallOnPause;
    [SerializeField]
    string[] functionsNamesToCallOnPause;

    //Deactivating pause
    [SerializeField]
    MonoBehaviour[] scriptsToCallOnUnpause;
    [SerializeField]
    string[] functionsNamesToCallOnUnpause;

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
