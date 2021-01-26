using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{

    PlayerControls controls;
    Animator uiAnimator;




    // ACTIVATING
    [Header("ACTIVATING")]
    [SerializeField] KeyCode pauseKey = KeyCode.Escape;
    [SerializeField] GameObject[] objectsToFreezeOnPause = null;
    [SerializeField] MonoBehaviour[] scriptsToFreezeOnPause = null;
    [SerializeField] MonoBehaviour[] scriptsToCallOnPause = null;
    [SerializeField] string[] functionsNamesToCallOnPause = null;




    // DEACTIVATING
    [Header("DEACTIVATING")]
    [SerializeField] MonoBehaviour[] scriptsToCallOnUnpause = null;
    [SerializeField] string[] functionsNamesToCallOnUnpause = null;



    // CURSOR
    [Header("CURSOR")]
    public bool cursorLocked = false;










    // BASE FUNCTIONS
    // Use this for initialization
    private void Awake()
    {
        controls = GameManager.Instance.Controls;
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    void Start()
    {
        uiAnimator = GetComponent<Animator>();
        lockCursor(cursorLocked);
    }

    // Update is called once per graphic frame
    void Update()
    {
        if (controls.Duel.Pause.triggered)
        {
            Pause();
        }
    }





    // PAUSE ON / OFF
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






    // CURSOR
    void lockCursor(bool state)
    {
        if (state)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;

        Cursor.visible = !state;
    }





    // FREEZE
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
