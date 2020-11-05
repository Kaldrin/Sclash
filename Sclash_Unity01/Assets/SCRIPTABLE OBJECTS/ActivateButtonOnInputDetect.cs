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







    // Update is called once per frame
    void Update()
    {
        if (enabled && isActiveAndEnabled && Mathf.Abs(Input.GetAxis(axisToCheck)) > 0.1f)
        {
            if (superiorOrInferior)
            {
                if (Input.GetAxis(axisToCheck) < valueToCheck)
                    hasBeenChecked = false;


                if (Input.GetAxis(axisToCheck) > valueToCheck && !hasBeenChecked)
                {
                    hasBeenChecked = true;
                    buttonToActivate.onClick.Invoke();
                }
            }
            else
            {
                if (Input.GetAxis(axisToCheck) > valueToCheck)
                    hasBeenChecked = false;


                if (Input.GetAxis(axisToCheck) < valueToCheck && !hasBeenChecked)
                {
                    hasBeenChecked = true;
                    buttonToActivate.onClick.Invoke();
                }
            }
        }
    }
}
