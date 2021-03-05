using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

// This script allows for activating a designated button depending on an axis input, independently of any menu browser
// OPTIMIZED
public class ActivateButtonOnInputDetect : MonoBehaviour
{
    [SerializeField] string axisToCheck = "MenuSecondary";
    [SerializeField] InputAction action;
    [SerializeField] Button buttonToActivate = null;
    [SerializeField] float valueToCheck = 0.5f;
    [SerializeField] bool superiorOrInferior = false;
    bool hasBeenChecked = false;




    PlayerControls controls;
    
    private void OnEnable()
    {
        action.Enable();
    }

    private void OnDisable()
    {
        action.Disable();
    }

    private void Start()
    {
        action.started += (ctx) =>
        {
            Debug.Log("Started");
            buttonToActivate.onClick.Invoke();
        };
    }


    // Update is called once per frame
    void Update()
    {/*
        if (enabled && isActiveAndEnabled && controls.Menu.Menusecondary.triggered)
        {
            if (superiorOrInferior)
            {
                if (controls.Menu.Menusecondary.triggered)
                    hasBeenChecked = false;


                if (controls.Menu.Menusecondary.triggered && !hasBeenChecked)
                {
                    hasBeenChecked = true;
                    buttonToActivate.onClick.Invoke();
                }
            }
            else
            {
                if (controls.Menu.Menusecondary.triggered)
                    hasBeenChecked = false;


                if (controls.Menu.Menusecondary.triggered && !hasBeenChecked)
                {
                    hasBeenChecked = true;
                    buttonToActivate.onClick.Invoke();
                }
            }
        }*/
    }
}
