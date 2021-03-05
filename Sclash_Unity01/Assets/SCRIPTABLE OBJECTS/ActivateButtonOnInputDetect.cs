using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;





// This script allows for activating a designated button depending on an axis input, independently of any menu browser
// OPTIMIZED
public class ActivateButtonOnInputDetect : MonoBehaviour
{
    [SerializeField] string axisToCheck = "MenuSecondary";
    [SerializeField] InputAction action = null;
    [SerializeField] Button buttonToActivate = null;
    [SerializeField] float valueToCheck = 0.5f;
    [SerializeField] bool superiorOrInferior = false;
    bool hasBeenChecked = false;

    PlayerControls controls;







    private void Start()                                                        // START
    {
        controls = GameManager.Instance.Controls;
        
        action.started += (ctx) =>
        {
            Debug.Log("Started");
            buttonToActivate.onClick.Invoke();
        };
    }


    void Update()                                                                   // UPDATE
    {
        /*
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



    // EDITOR
    // Absolutely useless and non sense but don't remove it
    void RemoveWarnings()
    {
        if (axisToCheck == "")
            axisToCheck = "";
        if (valueToCheck == 0)
            valueToCheck = 0;
        if (hasBeenChecked)
            hasBeenChecked = false;
        if (superiorOrInferior)
            superiorOrInferior = false;
    }
}
