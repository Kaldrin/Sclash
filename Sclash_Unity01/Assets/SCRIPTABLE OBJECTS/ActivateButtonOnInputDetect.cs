using UnityEngine;
using UnityEngine.UI;

// This script allows for activating a designated button depending on an axis input, independently of any menu browser
// OPTIMIZED
public class ActivateButtonOnInputDetect : MonoBehaviour
{
    [SerializeField] string axisToCheck = "MenuSecondary";
    [SerializeField] Button buttonToActivate = null;
    [SerializeField] float valueToCheck = 0.5f;
    [SerializeField] bool superiorOrInferior = false;
    bool hasBeenChecked = false;




    PlayerControls controls;
    private void Start()
    {
        controls = GameManager.Instance.Controls;
    }


    // Update is called once per frame
    void Update()
    {
        if (enabled && isActiveAndEnabled && Mathf.Abs(controls.Menu.Menusecondary.ReadValue<float>()) > 0.1f)
        {
            if (superiorOrInferior)
            {
                if (controls.Menu.Menusecondary.ReadValue<float>() < valueToCheck)
                    hasBeenChecked = false;


                if (controls.Menu.Menusecondary.ReadValue<float>() > valueToCheck && !hasBeenChecked)
                {
                    hasBeenChecked = true;
                    buttonToActivate.onClick.Invoke();
                }
            }
            else
            {
                if (controls.Menu.Menusecondary.ReadValue<float>() > valueToCheck)
                    hasBeenChecked = false;


                if (controls.Menu.Menusecondary.ReadValue<float>() < valueToCheck && !hasBeenChecked)
                {
                    hasBeenChecked = true;
                    buttonToActivate.onClick.Invoke();
                }
            }
        }
    }
}
