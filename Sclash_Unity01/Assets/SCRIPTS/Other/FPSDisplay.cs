using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;






// HEADER
// Reusable script
// For Sclash
// OPTIMIZED

// REQUIREMENTS
// Requires Text Mesh Pro package

/// <summary>
/// Script that allows to display the current FPS of the game. You cna adjust the frequency. It will only display FPS in editor or in development build, so you don't have to worry about it.
/// </summary>

// VERSION
// Originally made for Unity 2019.14
public class FPSDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textMeshProUGUIToDiplay = null;
    [SerializeField] TextMeshPro textMeshproToDisplay = null;
    [SerializeField] Text textToDisplay = null;
    [SerializeField] TextMesh textMeshToDisplay = null;
    [SerializeField] float customRefreshFrequency = 5;
    [SerializeField] bool useCustomFrequencyCoroutine = true;




    #region FUNCTIONS
    private void Awake()                                                                                              // AWAKE
    {
        GetComponents();
    }

    private void Start()                                                                                                // START
    {
        if (useCustomFrequencyCoroutine)
        {
            if (Debug.isDebugBuild || Application.isEditor)
                StartCoroutine(RefreshCoroutine());
            else
                RemoveDisplay();
        }
    }


    private void Update()                                                                                                   // UPDATE
    {
        if (enabled && isActiveAndEnabled)
            if (!useCustomFrequencyCoroutine)
            {
                if (Debug.isDebugBuild || Application.isEditor)
                    DisplayFPS();
                else
                    RemoveDisplay();
            }
    }

    IEnumerator RefreshCoroutine()                                                                                          // REFRESH COROUTINE
    {
        if (enabled && isActiveAndEnabled)
            while (true)
            {
                DisplayFPS();
                yield return new WaitForSecondsRealtime(1 / customRefreshFrequency);
            }
    }




    public void DisplayFPS()                                                                                                // DISPLAY VERSION
    {
        // If components are missing, try to find them
        GetComponents();


        // Gets the version
        int fps = Mathf.FloorToInt( 1 / Time.deltaTime);
        string fpsToDisplay = fps.ToString();

        // Displays the version of the game in the text component(s)
        if (textMeshProUGUIToDiplay != null)
            textMeshProUGUIToDiplay.text = fpsToDisplay;
        if (textMeshproToDisplay != null)
            textMeshproToDisplay.text = fpsToDisplay;
        if (textToDisplay != null)
            textToDisplay.text = fpsToDisplay;
        if (textMeshToDisplay != null)
            textMeshToDisplay.text = fpsToDisplay;
    }

    public void RemoveDisplay()                                                                                                 // REMOVE DISPLAY
    {
        if (textMeshProUGUIToDiplay != null)
            textMeshProUGUIToDiplay.enabled = false;
        if (textMeshproToDisplay != null)
            textMeshproToDisplay.enabled = false;
        if (textToDisplay != null)
            textToDisplay.enabled = false;
        if (textMeshToDisplay != null)
            textMeshToDisplay.text = "";

        this.enabled = false;
    }







    // SECONDARY
    // Check if it's possible to find the components automatically if they are missing
    void GetComponents()
    {
        if (textMeshProUGUIToDiplay == null && GetComponent<TextMeshProUGUI>())
            textMeshProUGUIToDiplay = GetComponent<TextMeshProUGUI>();
        if (textMeshproToDisplay == null && GetComponent<TextMeshPro>())
            textMeshproToDisplay = GetComponent<TextMeshPro>();
        if (textToDisplay == null && GetComponent<Text>())
            textToDisplay = GetComponent<Text>();
        if (textMeshToDisplay == null && GetComponent<TextMesh>())
            textMeshToDisplay = GetComponent<TextMesh>();
    }







    // EDITOR
    private void OnDrawGizmosSelected()                                                                                     // ON DRAW GIZMOS SELECTED
    {
        // Ergonomy stuff
        // Tries to find the components as soon as it's in the scene before the user has ton drag'n drop them in the Serialized Fields
        GetComponents();
    }
    #endregion
}
